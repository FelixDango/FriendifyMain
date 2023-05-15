using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public ProfileController(FriendifyContext context, UserManager<User> userManager,
                                 IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // The get action allows an authenticated user to get their own profile or an admin to get any profile by id
        [HttpGet("{id}")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Get(int id) // Indicate that the id is bound from route data
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is an admin or requesting their own profile
            if (currentUser != null && (currentUser.IsAdmin || currentUser.Id == id))
            {
                // Get the user by id from the database context
                var user = await _context.Users.FindAsync(id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Return a 200 OK response with the user data
                return Ok(user);
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }

        // The update action allows an authenticated user to update their own profile or an admin to update any profile by id
        [HttpPut("{id}")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(IdentityError[]), 400)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Update(int id, [FromBody] User model) // Indicate that the id is bound from route data and the model is bound from form data
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is an admin or updating their own profile
            if (currentUser != null && (currentUser.IsAdmin || currentUser.Id == id))
            {
                // Get the user by id from the database context
                var user = await _context.Users.FindAsync(id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Use AutoMapper to update the user properties from the model
                _mapper.Map(model, user);

                // Update the user in the database context and save changes
                var result = await _userManager.UpdateAsync(user);

                // Check if the update was successful
                if (result.Succeeded)
                {
                    // Return a 200 OK response with the updated user data
                    return Ok(user);
                }

                // If not, return a 400 Bad Request response with the errors
                return BadRequest(result.Errors);
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }
        // The delete action allows an authenticated user to delete their own profile or an admin to delete any profile by id
        [HttpDelete("{id}")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Delete(int id) // Indicate that the id is bound from route data
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is an admin or deleting their own profile
            if (currentUser != null && (currentUser.IsAdmin || currentUser.Id == id))
            {
                // Get the user by id from the database context
                var user = await _context.Users.FindAsync(id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Delete the user from the database context and save changes
                await _userManager.DeleteAsync(user);

                // Return a 200 OK response
                return Ok();
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }

        // The get action allows an authenticated user to get a list of all profiles or a filtered list by name or role
        [HttpGet]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<User>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> Get([FromQuery] string name, [FromQuery] string role) // Indicate that the name and role are bound from query string
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is an admin or a moderator
            if (currentUser != null && (currentUser.IsAdmin || currentUser.IsModerator))
            {
                // Get all users from the database context
                var users = await _context.Users.ToListAsync();

                // Filter users by name if provided
                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name)).ToList();
                }

                // Filter users by role if provided
                if (!string.IsNullOrEmpty(role))
                {
                    users = users.Where(u => u.AssignedRoles.Any(r => r.Role.Name == role)).ToList();
                }

                // Return a 200 OK response with the filtered users list
                return Ok(users);
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }
    }

}