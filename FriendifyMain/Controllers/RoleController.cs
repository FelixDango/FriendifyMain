using AutoMapper;
using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        // Inject the database context, the user manager and the role manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public RoleController(FriendifyContext context, UserManager<User> userManager,
                              RoleManager<Role> roleManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

       

        //assign role to user
        [HttpPost("assignrole")]
        [Authorize] // Require admin role
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> AssignRole(int userId, string roleName) // Indicate that the userId and roleName are bound from form data
        {
            try
            {
                
                // Get the current user from the user manager 
                var currentUser = await _userManager.GetUserAsync(User);

                // return 403 if current user is not Admin
                if (currentUser == null || !currentUser.IsAdmin)
                {
                    return Forbid();
                }


                // Get the user by id from the user manager
                var user = await _userManager.FindByIdAsync(userId.ToString());

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                if (roleName.ToUpper() != "ADMIN" && roleName.ToUpper() != "MODERATOR")
                {
                    return NotFound("Role not found."); // Return a 404 not found response with an error message
                }

                user.IsAdmin = roleName.ToUpper() == "ADMIN";
                user.IsModerator = roleName.ToUpper() == "MODERATOR";

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Return a 200 OK response with a message
                    return Ok($"User {user.UserName} assigned to role {roleName}.");
                }

                // If not, return a 400 Bad Request response with the errors
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }
        }

        // The remove action allows an admin to remove a role from a user
        [HttpPost("removerole")]
        [Authorize] // Require admin role
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> RemoveRole(int userId, string roleName) // Indicate that the userId and roleName are bound from form data
        {
            try
            {

                // Get the current user from the user manager 
                var currentUser = await _userManager.GetUserAsync(User);

                // return 403 if current user is not Admin
                if (currentUser == null || !currentUser.IsAdmin)
                {
                    return Forbid();
                }

                // Get the user by id from the user manager
                var user = await _userManager.FindByIdAsync(userId.ToString());


                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found."); // Return a 404 not found response with an error message
                }

                if (roleName.ToUpper() != "ADMIN" && roleName.ToUpper() != "MODERATOR")
                {
                    return NotFound("Role not found."); // Return a 404 not found response with an error message
                }

                if (roleName.ToUpper() == "ADMIN")
                {
                    user.IsAdmin = false;
                }

                if (roleName.ToUpper() == "MODERATOR")
                {
                    user.IsModerator = false;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Return a 200 OK response with a message
                    return Ok($"User {user.UserName} removed from role {roleName}.");
                }

                // If not, return a 400 Bad Request response with the errors
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }
        }


    }
}
