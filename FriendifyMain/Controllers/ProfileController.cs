using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FriendifyMain.ViewModels;

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

            if (currentUser == null || _context == null) { return BadRequest(); }

            // Check if the user is suspended
            if (currentUser.Suspended)
            {
                return BadRequest("You are suspended"); // Return a bad request response with an error message
            }

            // Load the related data explicitly
            await _context.Entry(currentUser).Collection(u => u.FollowedBy).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Follows).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.AssignedRoles).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Posts).LoadAsync();

            await _context.Posts
                    .Include(p => p.Pictures)
                    .Include(p => p.Videos)
                    .Include(p => p.Comments)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();


            // Check if the current user is an admin or requesting their own profile
            if (currentUser.IsAdmin || currentUser.Id == id)
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateViewModel model)
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || _context == null) { return BadRequest(); }

            // Check if the current user is an admin or updating their own profile
            if (currentUser.IsAdmin || currentUser.Id == id)
            {
                // Get the user by id from the database context
                var user = await _context.Users.FindAsync(id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found.");
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
                    users = users.Where(u => u.UserName != null && (u.UserName.Contains(name) || u.FirstName.Contains(name) || u.LastName.Contains(name))).ToList();
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

        // The follow action allows the current user to follow another user by their id
        [HttpPost("{id}/follow")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Follow(int id) // Indicate that the id is bound from route data
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null || _context == null) { return BadRequest(); }

                // Load the related data explicitly
                await _context.Entry(currentUser).Collection(u => u.FollowedBy).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Follows).LoadAsync();

                // Check if the current user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }

                // Get the other user by their id from the database context
                var otherUser = await _context.Users.FindAsync(id);

                // Check if the other user exists
                if (otherUser == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Check if the current user is already following the other user
                if (currentUser.Follows.Contains(otherUser))
                {
                    return Ok("You are already following this user."); // Return a 200 OK response with a message
                }

                // If not, add the other user to the follows list of the current user and save changes to database context
                currentUser.Follows.Add(otherUser);
                await _context.SaveChangesAsync();

                // Return a 200 OK response with a message
                return Ok($"You are now following {otherUser.UserName}.");
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }

        // The unfollow action allows the current user to unfollow another user by their id
        [HttpPost("{id}/unfollow")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Unfollow(int id) // Indicate that the id is bound from route data
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null || _context == null) { return BadRequest(); }

                // Load the related data explicitly
                await _context.Entry(currentUser).Collection(u => u.FollowedBy).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Follows).LoadAsync();

                // Check if the current user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }

                // Get the other user by their id from the database context
                var otherUser = await _context.Users.FindAsync(id);

                // Check if the other user exists
                if (otherUser == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                // Check if the current user is following the other user
                if (!currentUser.Follows.Contains(otherUser))
                {
                    return Ok("You are not following this user."); // Return a 200 OK response with a message
                }

                // If yes, remove the other user from the follows list of the current user and save changes to database context
                currentUser.Follows.Remove(otherUser);
                await _context.SaveChangesAsync();

                // Return a 200 OK response with a message
                return Ok($"You have unfollowed {otherUser.UserName}.");
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }
    }

}