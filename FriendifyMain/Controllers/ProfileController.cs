using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{id}/CriticalData")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> GetCrit(int id) // Indicate that the id is bound from route data
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
            await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Posts).LoadAsync();

            await _context.Posts
                            .Include(p => p.Pictures)
                            .Include(p => p.Videos)
                            .Include(p => p.Comments)
                            .OrderByDescending(p => p.Date)
                            .ToListAsync();

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == id); // Filter by id


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

        // The get action allows an authenticated user to get their own profile or an admin to get any profile by id
        [HttpGet("{id}/view")]
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
            await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();
            await _context.Entry(currentUser).Collection(u => u.Posts).LoadAsync();

            await _context.Posts
                            .Include(p => p.Pictures)
                            .Include(p => p.Videos)
                            .Include(p => p.Comments)
                            .OrderByDescending(p => p.Date)
                            .ToListAsync();

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == id); // Filter by id



            // Get the user by id from the database context
            var user = await _context.Users.FindAsync(id);

            // Check if the user exists
            if (user == null)
            {
                return NotFound("User not found."); // Return a 404 not found response with an error message
            }

            // Remove Critical fields from response
            user.PasswordHash = "Censored";
            user.Email = "Censored";
            user.Address = "Censored";
            user.Country = "Censored";
            user.Likes = new();
            user.Comments = new();
            user.NormalizedEmail = "Censored";
            user.SecurityStamp = "Censored";
            user.City = "Censored";
            user.PhoneNumber = "Censored";
            user.Images = new();
            user.Messages = new();
            user.Videos = new();

            // Return a 200 OK response with the user data
            return Ok(user);

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

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == id); // Filter by id

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

                if (model != null && model.PictureUrl != null)
                {
                    user.Picture.Url = model.PictureUrl;
                }

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

        // The get action allows an authenticated user to get a list of all profiles or a filtered list by name
        [HttpGet]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<User>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> Get([FromQuery] string name) // Indicate that the name from query string
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
                    users = users.Where(u => u.UserName != null && (u.UserName.ToUpper().Contains(name.ToUpper()) || u.FirstName.ToUpper().Contains(name.ToUpper()) || u.LastName.ToUpper().Contains(name.ToUpper()))).ToList();
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
                await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();

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
                if (currentUser.Following.Select(e => e.UserId == otherUser.Id).FirstOrDefault())
                {
                    return Ok("You are already following this user."); // Return a 200 OK response with a message
                }

                // If not, add the other user to the follows list of the current user and save changes to database context
                currentUser.Following.Add(new Follower() { UserId = otherUser.Id, FollowerId = currentUser.Id });
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
                await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();

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
                if (!currentUser.Following.Select(e => e.UserId == otherUser.Id).FirstOrDefault())
                {
                    return Ok("You are not following this user."); // Return a 200 OK response with a message
                }

                // If yes, remove the other user from the follows list of the current user and save changes to database context
                currentUser.Following = currentUser.Following.Where(e => e.UserId != otherUser.Id).ToList(); // Remove the other user from the following list of the current user
                otherUser.Followers = otherUser.Followers.Where(e => e.UserId != currentUser.Id).ToList(); // Remove the current user from the followers list of the other user

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


