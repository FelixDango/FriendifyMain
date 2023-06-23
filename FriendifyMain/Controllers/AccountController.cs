using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Enable automatic model validation and error handling
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper; // Declare the _mapper variable
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;


        // Create a symmetric security key from the secret key
        private readonly SymmetricSecurityKey _securityKey;

        // Create a signing credentials object from the security key
        private SigningCredentials _signingCredentials;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                                 IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

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
            user.Picture = new() { Id = 0, UserId = user.Id, User = user, Url = "assets/media/default.png" };

            // Add logging statements to track the user creation process
            Console.WriteLine("Creating user: " + user.UserName);

            var result = await _userManager.CreateAsync(user);
            Console.WriteLine("result: " + result);


            // Check if the user creation was successful
            if (result.Succeeded)
            {
                // Return a 200 OK response with the user data and the JWT token
                return Ok(new { user = user, token = GenerateToken(user) });
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

            // Get the user by their username from the user manager
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                // User not found
                return NotFound("User not found.");
            }

            // Check if the password is valid
            var passwordResult = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordResult)
            {
                // Password is invalid
                return Unauthorized("Invalid password.");
            }

            // Return a 200 OK response with the user data and the JWT token
            return Ok(new { user = user, token = GenerateToken(user) });

        }


        // The logout action allows a signed-in user to sign out from their account
        [HttpPost("logout")]
        [ProducesResponseType(200)] // Specify possible response status code
        public async Task<IActionResult> Logout()
        {

            // Return a 200 OK response
            return Ok();
        }

        // A helper method that can generate a JWT token for a given user
        private string GenerateToken(User user)
        {
            // Create a list of claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "User")
            };

            // Create a JWT token for the user with the specified claims and expiration time
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:JwtIssuer"],
                audience: _configuration["Jwt:JwtAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: _signingCredentials);

            // Write the token to a string and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            Console.WriteLine(tokenString);
            return tokenString;
        }
    }
}
