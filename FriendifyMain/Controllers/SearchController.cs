using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // The user action allows an authenticated user to search other users by name (Username, Firstname or Lastname) or role
        [HttpGet("finduser")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<User>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> FindUser([FromBody] string name, [FromBody] string role) // Indicate that the name (Username, Firstname or Lastname) and role are bound from query string
        {
            // Get all users from the database context
            var users = await _context.Users.ToListAsync();

            // Filter users by name (Username, Firstname or Lastname) if provided
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

        // The post action allows an authenticated user to search posts by content or date
        [HttpGet("finduser")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<Post>), 200)] // Specify possible response type and status code
        public async Task<IActionResult> FindPost([FromBody] string content, [FromBody] DateTime? date) // Indicate that the content and date are bound from query string
        {
            // Get all posts from the database context
            var posts = await _context.Posts.ToListAsync();

            // Filter posts by content if provided
            if (!string.IsNullOrEmpty(content))
            {
                posts = posts.Where(p => p.Content.Contains(content)).ToList();
            }

            // Filter posts by date if provided
            if (date.HasValue)
            {
                posts = posts.Where(p => p.Date.Date == date.Value.Date).ToList();
            }

            // Return a 200 OK response with the filtered posts list
            return Ok(posts);
        }
    }
}
