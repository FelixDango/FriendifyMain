using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FriendifyMain.ViewModels;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public SettingsController(FriendifyContext context, UserManager<User> userManager,
                                 IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // The change password action allows an authenticated user to change their password
        [HttpPost("changepassword")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(IdentityError[]), 400)] // Specify possible response type and status code
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model) // Indicate that the model is bound from form data
        {
            // Validate the model using data annotations and custom logic
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the user exists
                if (currentUser == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Change the password of the current user using the user manager and check if it was successful
                var result = await _userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    // Return a 200 OK response with the updated user data
                    return Ok(currentUser);
                }

                // If not, return a 400 Bad Request response with the errors
                return BadRequest(result.Errors);
            }

            // If the model is not valid, return a 400 Bad Request response with validation errors
            return BadRequest(ModelState);
        }

    }
}
