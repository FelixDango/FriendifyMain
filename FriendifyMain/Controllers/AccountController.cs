using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // The register action allows a new user to create an account
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate the model using data annotations and custom logic
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password) && model.Password == model.ConfirmPassword)
            {
                // Create a new user with the given username and password
                var user = new User { Username = model.Username };

                // Add logging statements to track the user creation process
                Console.WriteLine("Creating user: " + user.Username);

                var result = await _userManager.CreateAsync(user, model.Password);
                Console.WriteLine("result: " + result);


                // Check if the user creation was successful
                if (result.Succeeded)
                {
                    // Sign in the user using a cookie
                    await _signInManager.SignInAsync(user, isPersistent: false);

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

            // Log the validation errors if the model is not valid
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Model validation error: " + error.ErrorMessage);
            }

            // If the model is not valid, return a 400 Bad Request response with the validation errors
            return BadRequest(ModelState);
        }

        // The login action allows an existing user to sign in using their username and password
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validate the model using data annotations and custom logic
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
            {
                // Sign in the user using a cookie and check if it was successful
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Get the user by their username from the user manager
                    var user = await _userManager.FindByNameAsync(model.Username);

                    // Return a 200 OK response with the user data
                    return Ok(user);
                }

                // If not, return a 401 Unauthorized response with an error message
                return Unauthorized("Invalid login attempt.");
            }

            // If the model is not valid, return a 400 Bad Request response with the validation errors
            return BadRequest(ModelState);
        }

        // The logout action allows a signed-in user to sign out from their account
        [Authorize] // Require authentication
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user from the cookie
            await _signInManager.SignOutAsync();

            // Return a 200 OK response
            return Ok();
        }
    }
}
