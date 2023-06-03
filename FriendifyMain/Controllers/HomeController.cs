﻿using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(FriendifyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // The index action returns the home page with the latest posts from the users that the current user follows
        [Authorize] // Require authentication
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> Index()
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);
                // Load the related data explicitly
                await _context.Entry(currentUser).Collection(u => u.FollowedBy).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Follows).LoadAsync();

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }


                // Get the list of users that the current user follows
                var followedUsers = currentUser.Follows.Select(f => f.Id).ToList();

                // Get the latest posts from the followed users, ordered by date in descending order
                var posts = await _context.Posts
                    .Where(p => followedUsers.Contains(p.UserId))
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();

                // Return an OK response with the posts as the data
                return Ok(posts);



            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }

        // The create action allows the current user to create a new post with optional pictures and videos
        [Authorize] // Require authentication
        [HttpPost] // Only respond to POST requests
        [HttpPost("createpost")] // Only respond to POST requests with a PostId parameter in the route 
        public async Task<ActionResult<Post>> Create([FromBody] PostViewModel postModel)
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }

                // Validate the post model using data annotations and custom logic
                if (ModelState.IsValid && !string.IsNullOrEmpty(postModel.Content))
                {
                    var post = new Post { Content = postModel.Content };
                    // Set the post properties that are not provided by the user input
                    post.Id = 0; // The id will be generated by the database automatically
                    post.UserId = currentUser.Id; // The user is the current user
                    post.User = currentUser;
                    post.Date = DateTime.Now; // The date is the current date and time
                    post.LikedBy = new List<User>(); // The liked by list is initially empty
                    post.Comments = new List<Comment>(); // The comments list is initially empty
                    post.Pictures = new List<Picture>();
                    post.Videos = new List<Video>();
                    postModel.Pictures.ForEach(e => post.Pictures.Add(new Picture { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));
                    postModel.Videos.ForEach(e => post.Videos.Add(new Video { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));

                    // Add the post to the database context and save changes
                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();

                    // Return a created at action response with the post as the data and a location header pointing to the index action 
                    return CreatedAtAction(nameof(Index), post);
                }

                // If the model is not valid, return a bad request response with the model state errors as the data 
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }

        // The like action allows the current user to like or unlike a post by its id
        [Authorize] // Require authentication
        [HttpPost("{PostId}/like")] // Only respond to POST requests with a PostId parameter in the route 
        public async Task<IActionResult> Like(int PostId)
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message 
                }

                // Get the post by its id from the database context
                var post = await _context.Posts.FindAsync(PostId);

                // Check if the post exists
                if (post == null)
                {
                    return NotFound(); // Return a 404 not found response 
                }

                //if post is new let's create a list of likedby 
                if (post.LikedBy == null)
                {
                    post.LikedBy = new();
                }

                // Check if the current user has already liked the post 
                if (post.LikedBy.Contains(currentUser))
                {
                    // If yes, remove the current user from the liked by list 
                    post.LikedBy.Remove(currentUser);
                }
                else
                {
                    // If no, add the current user to the liked by list 
                    post.LikedBy.Add(currentUser);
                }

                // Save changes to database context 
                await _context.SaveChangesAsync();

                // Return a no content response indicating success 
                return NoContent();
            }
            catch (Exception ex)
            {
                //Handle any possible exceptions
                return StatusCode(500, ex.Message); //Return an internal server error response with error message
            }
        }

        //The comment action allows current user to add comment to a post by its id and comment text 
        [Authorize] //Require authentication
        [HttpPost("{PostId}/comment")]  //Only respond to POST requests with PostId parameter in route 
        public async Task<IActionResult> Comment(int PostId, string text)
        {
            try
            {
                //Get current user from user manager
                var currentUser = await _userManager.GetUserAsync(User);

                //Check if user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); //Return bad request response with error message
                }

                //Get post by its id from database context
                var post = await _context.Posts.FindAsync(PostId);

                //Check if post exists and comment text is not empty or null
                if (post == null || string.IsNullOrEmpty(text))
                {
                    return NotFound(); //Return 404 not found response
                }

                //Create new comment with given text and current date and time
                var comment = new Comment()
                {
                    Text = text,
                    Date = DateTime.Now,
                    UserId = currentUser.Id,
                    PostId = post.Id,
                    Id = 0,
                };

                //init Comments list in new posts
                if (post.Comments == null)
                {
                    post.Comments = new();
                }
                //Add comment to comments list of post
                post.Comments.Add(comment);
                //Save changes to database context
                await _context.SaveChangesAsync();

                //Return created at action response with comment as data and location header pointing to index action
                return CreatedAtAction(nameof(Index), comment);
            }
            catch (Exception ex)
            {
                //Handle any possible exceptions
                return StatusCode(500, ex.Message); //Return internal server error response with error message
            }
        }

        //The delete action allows current user to delete a post by its id if they are owner or moderator  
        [Authorize] //Require authentication
        [HttpDelete("{PostId}/deletepost")]  //Only respond to DELETE requests with PostId parameter in route  
        public async Task<IActionResult> Delete(int PostId)
        {
            try
            {
                //Get current user from user manager
                var currentUser = await _userManager.GetUserAsync(User);

                //Check if user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); //Return bad request response with error message
                }

                // Get the post by its id from the database context
                var post = await _context.Posts.FindAsync(PostId);

                // Check if the post exists
                if (post == null)
                {
                    return NotFound(); // Return a 404 not found response
                }

                // Load the related data explicitly
                await _context.Entry(post).Collection(p => p.Comments).LoadAsync();
                await _context.Entry(post).Collection(p => p.Pictures).LoadAsync();
                await _context.Entry(post).Collection(p => p.Videos).LoadAsync();

                //Check if current user is owner or moderator of post
                if (post.UserId == currentUser.Id || currentUser.IsModerator || currentUser.IsAdmin)
                {
                    //If yes, remove post from database context and save changes
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();

                    //Redirect to index action to show updated posts list
                    return RedirectToAction(nameof(Index));
                }

                //If no, return 403 forbidden response
                return Forbid();
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); //Return an internal server error response with the error message
            }
        }

        // The edit action allows the owner or admin or moderator to edit a post by its id
        [Authorize] // Require authentication
        [HttpGet("{PostId}/getedit")] // Only respond to GET requests with a PostId parameter in the route
        public async Task<ActionResult<Post>> Edit(int PostId)
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }

                // Get the post by its id from the database context
                var post = await _context.Posts.FindAsync(PostId);

                // Check if the post exists
                if (post == null)
                {
                    return NotFound(); // Return a 404 not found response
                }

                // Load the related data explicitly
                await _context.Entry(post).Collection(p => p.Comments).LoadAsync();
                await _context.Entry(post).Collection(p => p.Pictures).LoadAsync();
                await _context.Entry(post).Collection(p => p.Videos).LoadAsync();

                // Check if the current user is the owner or admin or moderator of the post
                if (post.UserId == currentUser.Id || currentUser.IsAdmin || currentUser.IsModerator)
                {
                    post.Comments ??= new();
                    post.LikedBy ??= new();
                    post.Pictures ??= new();
                    post.Videos ??= new();
                    // If yes, return an OK response with the post as the data
                    return Ok(post);
                }

                // If no, return a 403 forbidden response
                return Forbid();
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }

        [Authorize] // Require authentication
        [HttpPut("{PostId}/putedit")] // Only respond to PUT requests with a PostId parameter in the route
        public async Task<IActionResult> Edit(int PostId, [FromBody] PostViewModel postModel)
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message
                }

                // Validate the post model using data annotations and custom logic
                if (ModelState.IsValid && !string.IsNullOrEmpty(postModel.Content))
                {
                    // Get the post by its id from the database context
                    var originalPost = await _context.Posts.FindAsync(PostId);

                    // Check if the original post exists and matches with the given post id
                    if (originalPost == null || originalPost.Id != PostId)
                    {
                        return NotFound(); // Return a 404 not found response
                    }

                    // Load the related data explicitly
                    await _context.Entry(originalPost).Collection(p => p.Comments).LoadAsync();
                    await _context.Entry(originalPost).Collection(p => p.Pictures).LoadAsync();
                    await _context.Entry(originalPost).Collection(p => p.Videos).LoadAsync();

                    // Check if the current user is the owner or admin or moderator of the original post
                    if (originalPost.UserId == currentUser.Id || currentUser.IsAdmin || currentUser.IsModerator)
                    {
                        // If yes, update the original post properties that can be edited by the user input
                        originalPost.Content = postModel.Content;
                        originalPost.Pictures = new();
                        originalPost.Videos = new();
                        postModel.Pictures.ForEach(e => originalPost.Pictures.Add(new Picture { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));
                        postModel.Videos.ForEach(e => originalPost.Videos.Add(new Video { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));

                        // Save changes to database context and reload original post to reflect any changes made by triggers or computed columns in database 
                        _context.Posts.Update(originalPost);
                        await _context.SaveChangesAsync();
                        await _context.Entry(originalPost).ReloadAsync();

                        // Return a no content response indicating success 
                        return NoContent();
                    }

                    // If no, return a 403 forbidden response 
                    return Forbid();
                }

                // If model is not valid, return a bad request response with model state errors as data 
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                //Handle any possible exceptions
                return StatusCode(500, ex.Message); //Return internal server error response with error message
            }
        }
    }
}