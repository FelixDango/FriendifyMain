﻿using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(FriendifyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get the total interactions (likes + comments + follows + posts) in the selected time 
        [HttpGet]
        [Authorize]
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Index(DateTime selectedTime)
        {
            try
            {
                // Get the current user from the user manager 
                var currentUser = await _userManager.GetUserAsync(User);

                // return 403 if current user is not Admin
                if (currentUser == null || (!currentUser.IsAdmin && !currentUser.IsModerator))
                {
                    return Forbid();
                }

                var totalInteractions = await _context.Likes.CountAsync(l => l.DateTime >= selectedTime) +
                                    await _context.Comments.CountAsync(c => c.Date >= selectedTime) +
                                    await _context.Followers.CountAsync(f => f.DateTime >= selectedTime) +
                                    await _context.Posts.CountAsync(p => p.Date >= selectedTime);

                // Get total posts in selected time range
                var totalPosts = await _context.Posts.CountAsync(p => p.Date >= selectedTime);

                // Get the total accounts created in the selected time range
                var totalAccountsInTimespan = await _context.Users.CountAsync(u => u.RegisteredAt >= selectedTime);

                // Get the total accounts
                var totalAccounts = await _context.Users.CountAsync();

                // Get list of all registration dates in selected time range
                var registrationDates = await _context.Users
                                              .Where(u => u.RegisteredAt >= selectedTime)
                                              .Select(u => u.RegisteredAt)
                                              .ToListAsync();

                // Get list of all post creation dates in selected time range
                var postsCreationDates = await _context.Posts
                                              .Where(p => p.Date >= selectedTime)
                                              .Select(p => p.Date)
                                              .ToListAsync();

                // Get the count of males and females users
                var maleCount = await _context.Users.CountAsync(u => u.Sex == Models.User.SexEnum.Male);
                var femaleCount = await _context.Users.CountAsync(u => u.Sex == Models.User.SexEnum.Female);

                // Get the average interactions of a user in the selected time range
                var averageInteractions = totalAccounts > 0 ? (double)totalInteractions / totalAccounts : 0;

                // Create an AdminData object with the data
                var data = new AdminData
                {
                    TotalPosts = totalPosts,
                    TotalInteractions = totalInteractions,
                    TotalAccounts = totalAccounts,
                    TotalAccountsInTimespan = totalAccountsInTimespan,
                    MaleCount = maleCount,
                    FemaleCount = femaleCount,
                    AverageInteractions = averageInteractions,
                    RegistrationDates = registrationDates,
                    PostsCreationDates = postsCreationDates
                };

                // Return the result 
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }


        }

    }
}