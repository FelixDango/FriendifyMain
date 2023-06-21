using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet("{username}/CriticalData")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> GetCrit(string username) // Indicate that the username is bound from route data
        {
            if (username is null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            // Get the current user from the user manager
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var selectedUser = await _userManager.FindByNameAsync(username);

            if (currentUser == null || selectedUser == null || _context == null) { return BadRequest(); }

            // Check if the user is suspended
            if (currentUser.Suspended)
            {
                return BadRequest("You are suspended"); // Return a bad request response with an error message
            }

            // Load the related data explicitly
            await _context.Entry(selectedUser).Collection(u => u.Followers).LoadAsync();
            await _context.Entry(selectedUser).Collection(u => u.Following).LoadAsync();
            await _context.Entry(selectedUser).Collection(u => u.Posts).LoadAsync();

            await _context.Posts
                            .Include(p => p.Pictures)
                            .Include(p => p.Videos)
                            .Include(p => p.Comments)
                            .Include(p => p.Likes)
                            .OrderByDescending(p => p.Date)
                            .Where(p => p.UserId == selectedUser.Id)
                            .ToListAsync();

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .Include(u => u.Images)
                     .Include(u => u.Videos)
                     .FirstOrDefaultAsync(u => u.Id == selectedUser.Id); // Filter by id


            // Check if the current user is an admin or requesting their own profile
            if (currentUser.IsAdmin || currentUser.Id == selectedUser.Id)
            {
                // Return a 200 OK response with the user data
                return Ok(selectedUser);
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }

        // The get action allows an authenticated user to get their own profile or an admin to get any profile by id
        [HttpGet("{username}/view")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Get(string username) // Indicate that the username is bound from route data
        {
            if (username is null)
            {
                throw new ArgumentNullException(nameof(username));
            }
            // Get the current user from the user manager
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var selectedUser = await _userManager.FindByNameAsync(username);

            if (currentUser == null || selectedUser == null || _context == null) { return BadRequest(); }

            // Check if the user is suspended
            if (currentUser.Suspended)
            {
                return BadRequest("You are suspended"); // Return a bad request response with an error message
            }

            // Load the related data explicitly
            await _context.Entry(selectedUser).Collection(u => u.Followers).LoadAsync();
            await _context.Entry(selectedUser).Collection(u => u.Following).LoadAsync();
            await _context.Entry(selectedUser).Collection(u => u.Posts).LoadAsync();

            await _context.Posts
                            .Include(p => p.Pictures)
                            .Include(p => p.Videos)
                            .Include(p => p.Comments)
                            .Include(p => p.Likes)
                            .OrderByDescending(p => p.Date)
                            .Where(p => p.UserId == selectedUser.Id)
                            .ToListAsync();

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == selectedUser.Id); // Filter by id

            // Remove Critical fields from response
            selectedUser.PasswordHash = "Censored";
            selectedUser.Email = "Censored";
            selectedUser.Address = "Censored";
            selectedUser.Country = "Censored";
            selectedUser.Likes = new();
            selectedUser.Comments = new();
            selectedUser.NormalizedEmail = "Censored";
            selectedUser.SecurityStamp = "Censored";
            selectedUser.City = "Censored";
            selectedUser.PhoneNumber = "Censored";
            selectedUser.Images = new();
            selectedUser.Messages = new();
            selectedUser.Videos = new();

            // Return a 200 OK response with the user data
            return Ok(selectedUser);

        }


        // The update action allows an authenticated user to update their own profile or an admin to update any profile by id
        [HttpPut("{username}/update")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(User), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(IdentityError[]), 400)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Update(string username, [FromForm] UpdateViewModel model)
        {
            if (username is null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            // Get the current user from the user manager
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var selectedUser = await _userManager.FindByNameAsync(username);

            if (currentUser == null || selectedUser == null || _context == null) { return BadRequest(); }

            await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == selectedUser.Id); // Filter by id

            // Check if the current user is an admin or updating their own profile
            if (currentUser.IsAdmin || currentUser.Id == selectedUser.Id)
            {
                if (model != null)
                {
                    // Use AutoMapper to update the user properties from the model
                    _mapper.Map(model, selectedUser);

                    if (model.PictureFile != null)
                    {
                        var pictureUrl = await SaveFile(model.PictureFile);
                        if (pictureUrl != null)
                        {
                            selectedUser.Picture = new Picture { Id = 0, Url = pictureUrl, User = selectedUser, UserId = selectedUser.Id };
                        }
                        else
                        {
                            return BadRequest("Failed to save picture file"); // Return a bad request response with an error message
                        }
                    }
                    // Update the user in the database context and save changes
                    var result = await _userManager.UpdateAsync(selectedUser);
                    // Check if the update was successful
                    if (result.Succeeded)
                    {
                        // Return a 200 OK response with the updated user data
                        return Ok(selectedUser);
                    }
                    else
                    {
                        // If not, return a 400 Bad Request response with the errors
                        return BadRequest(result.Errors);
                    }
                }
            }
            // If not, return a 403 forbidden response
            return Forbid();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> SaveFile(IFormFile file)
        {
            var filePath = Path.Combine("wwwroot", "assets", "media", file.FileName);
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Open a file stream for writing
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Copy the file to the file stream
                await file.CopyToAsync(fileStream);
            }

            var strippedFilePath = Path.Combine("assets", "media", file.FileName);


            // Return the file path
            return strippedFilePath;
        }

        // The delete action allows an authenticated user to delete their own profile or an admin to delete any profile by id
        [HttpDelete("{username}")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Delete(string username) // Indicate that the id is bound from route data
        {
            // Get the current user from the user manager
            if (User == null || username == null ||User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var selectedUser = await _userManager.FindByNameAsync(username);

            // Check if the current user is an admin or deleting their own profile
            if ((currentUser != null && selectedUser != null ) && (currentUser.IsAdmin || currentUser.Id == selectedUser.Id))
            {
                // Delete the user from the database context and save changes
                await _userManager.DeleteAsync(selectedUser);

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
        public async Task<IActionResult> FindByName([FromQuery] string name) // Indicate that the name from query string
        {
            // Get the current user from the user manager
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

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
        [HttpPost("{username}/follow")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Follow(string username) // Indicate that the id is bound from route data
        {
            try
            {
                // Get the current user from the user manager
                if (User == null || username == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

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
                var otherUser = await _userManager.FindByNameAsync(username);

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
                currentUser.Following.Add(new Follower() { UserId = otherUser.Id, FollowerId = currentUser.Id, DateTime = DateTime.Now });
                await _context.SaveChangesAsync();

                var response = new { Message = $"You have followed {otherUser.UserName}." };
                // Return a 200 OK response with a message
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }

        // The unfollow action allows the current user to unfollow another user by their id
        [HttpPost("{username}/unfollow")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Unfollow(string username) // Indicate that the id is bound from route data
        {
            try
            {
                // Get the current user from the user manager
                if (User == null || username == null ||User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

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
                var otherUser = await _userManager.FindByNameAsync(username);

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
                var response = new { Message = $"You have unfollowed {otherUser.UserName}." };
                // Return a 200 OK response with a message
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }
    }

}


