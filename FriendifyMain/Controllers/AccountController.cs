using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Enable automatic model validation and error handling
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                                 IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // The register action allows a new user to create an account
        [HttpPost("register")]
        [AllowAnonymous] // Allow anonymous access
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(IdentityError[]), 400)] // Specify possible response type and status code
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model) // Indicate that the model is bound from form data
        {
            // Create a new user with the given username and password
            var user = _mapper.Map<User>(model); // Use AutoMapper to convert the RegisterViewModel to a User model

            // Hash the password and assign it to the PasswordHash property
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);

            //Initialize user fields/property fields
            user.RegisteredAt = DateTime.Now;
            user.Following = new();
            user.Followers = new();
            user.Posts = new();
            user.Comments = new();
            user.Images = new();
            user.Videos = new();
            user.Messages = new();
            user.Likes = new();
            user.Biography = "Hey, let's get connected!";
            user.Picture = new() { Id = 0, UserId = user.Id, User = user, Url = "test.png" };

            // Add logging statements to track the user creation process
            Console.WriteLine("Creating user: " + user.UserName);

            var result = await _userManager.CreateAsync(user);
            Console.WriteLine("result: " + result);


            // Check if the user creation was successful
            if (result.Succeeded)
            {
                // Sign in the user using a cookie
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Generate the authentication token
                var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "Authentication");

                // Store the token in the database
                await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "Authentication", token);

                // Set the token in the response headers
                Response.Headers.Add("Authorization", $"Bearer {token}");

                // Return a 200 OK response with the user data
                return Ok(user);
            }

            // Log the errors if user creation failed
            foreach (var error in result.Errors)
            {
                Console.WriteLine("User creation error: " + error.Description);
            }

            // If not, return a 400 Bad Request response with the errors
            return BadRequest(result.Errors);
        }



        // The login action allows an existing user to sign in using their username and password
        [HttpPost("login")]
        [AllowAnonymous] // Allow anonymous access
        [ProducesResponseType(typeof(string), 401)] // Specify possible response type and status code
        [ProducesResponseType(typeof(ModelStateDictionary), 400)] // Specify possible response type and status code
        [Produces("application/json")] // Specify response content type
        public async Task<IActionResult> Login([FromBody] LoginViewModel model) // Indicate that the model is bound from form data
        {
            // Sign in the user using a cookie and check if it was successful
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            Console.WriteLine(result);
            if (result.Succeeded)
            {
                // Get the user by their username from the user manager
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    // User not found
                    return NotFound("User not found.");
                }

                // Get the token from the database
                var token = await _userManager.GetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "Authentication");
                
                // Check if the token is null or expired
                if (token == null || await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Authentication", token) == false)
                {
                    // Generate a new token and store it in the database
                    token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "Authentication");
                    Console.WriteLine($"new token {token}");
                    await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "Authentication", token);
                }

                // Set the token in the response headers
                Response.Headers.Add("Authorization", $"Bearer {token}");
                Response.Headers.Add("Access-Control-Expose-Headers", "Authorization");
                
                // sign in user
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Return a 200 OK response with the user data in the body and the token in the header
                return Ok(user);
            }

            // If not, return a 401 Unauthorized response with an error message
            return Unauthorized("Invalid login attempt.");
        }

        // The logout action allows a signed-in user to sign out from their account
        [Authorize] // Require authentication
        [HttpPost("logout")]
        [ProducesResponseType(200)] // Specify possible response status code
        public async Task<IActionResult> Logout()
        {
            // Sign out the user from the cookie
            await _signInManager.SignOutAsync();

            // Return a 200 OK response
            return Ok();
        }
    }
}
