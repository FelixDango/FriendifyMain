using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class SearchController : Controller
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public SearchController(FriendifyContext context, UserManager<User> userManager,
                                 IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // The user action allows an authenticated user to search other users by name (Username, Firstname or Lastname)
        [HttpGet("finduser")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<User>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> FindUser([FromQuery] string? name) // Indicate that the name (Username, Firstname or Lastname) from query string
        {
            if (name == null || name.Trim() == "")
            {
                return Ok(new List<User>());
            }

            // Get all users from the database context
            var users = await _context.Users.ToListAsync();

            foreach (var currentUser in users) {
                // Load the related data explicitly
                await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Posts).LoadAsync();

                await _context.Users
                         .Include(u => u.Picture) // Include the Picture navigation property
                         .FirstOrDefaultAsync(u => u.Id == currentUser.Id); // Filter by id
            }
            

            // Filter users by name (Username, Firstname or Lastname) if provided
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(u => u.UserName != null && (u.UserName.ToUpper().Contains(name.ToUpper()) || u.FirstName.ToUpper().Contains(name.ToUpper()) || u.LastName.ToUpper().Contains(name.ToUpper()))).ToList();
            }
            // Remove Critical fields from response
            foreach (var user in users)
            {
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
            }
            // Return a 200 OK response with the filtered users list
            return Ok(users);
        }

        // The post action allows an authenticated user to search posts by content 
        [HttpGet("findpost")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<Post>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> FindPost([FromQuery] string content) // Indicate that the content from query string
        {
            // Get all posts from the database context
            var posts = await _context.Posts
                    .Include(p => p.Pictures)
                    .Include(p => p.Videos)
                    .Include(p => p.Comments)
                    .Include(p => p.Likes)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();

            // Filter posts by content if provided
            if (!string.IsNullOrEmpty(content))
            {
                posts = posts.Where(p => p.Content.ToUpper().Contains(content.ToUpper())).ToList();
            }

            // Return a 200 OK response with the filtered posts list
            return Ok(posts);
        }
    }
}
