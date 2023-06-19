using FriendifyMain.Models;
using FriendifyMain.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : ControllerBase
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(FriendifyContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // The index action returns the home page with the latest posts from the users that the current user follows and their own posts 
        // add bearer authentication
        [Authorize] // Require authentication
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> Index()
        {
            try
            {
                // Get the current user from the user manager 
                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (currentUser == null || _context == null) { return BadRequest(); }

                // Load the related data explicitly 
                await _context.Entry(currentUser).Collection(u => u.Followers).LoadAsync();
                await _context.Entry(currentUser).Collection(u => u.Following).LoadAsync();



                // Check if the user is suspended 
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message 
                }

                // Get the list of users that the current user follows and add the current user to the list 
                var followedUsers = currentUser.Following.Select(f => f.UserId).ToList();
                followedUsers.Add(currentUser.Id);

                // Get the latest posts from the followed users and the current user, ordered by date in descending order 
                var posts = await _context.Posts
                    .Where(p => followedUsers.Contains(p.UserId))
                    .Include(p => p.Pictures)
                    .Include(p => p.Videos)
                    .Include(p => p.Comments)
                    .Include(p => p.Likes)
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

        // The GetAllPosts action returns the home page with all posts ordered by date for anonymous users
        [AllowAnonymous] // Allow access by non-authenticated users
        [HttpGet("getallposts")] // Accept only GET requests and append the route to the controller route
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts(int pageNumber = 1, int pageSize = 0)
        {
            try
            {
                // Get all posts ordered by date in descending order
                var posts = await _context.Posts
                    .Include(p => p.Pictures)
                    .Include(p => p.Videos)
                    .Include(p => p.Comments)
                    .Include(p => p.Likes)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();

                // Check if paging is enabled
                if (pageSize > 0)
                {
                    // Skip the previous pages and take only the current page
                    posts = posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                // Return an OK response with the posts as the data
                return Ok(posts.ToList());
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
        }


        // The create action allows the current user to create a new post with optional pictures and videos
        [Authorize] // Require authentication
        [HttpPost("createpost")] // Accept only POST requests and append the route to the controller route
        public async Task<ActionResult<Post>> Create([FromForm] PostViewModel postModel)
        {
            try
            {
                // Get the current user from the user manager
                if (User.Identity.IsAuthenticated)
                {
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    
                    if (currentUser == null)
                    {
                        return BadRequest();
                    }

                    // Check if the user is suspended
                    if (currentUser.Suspended)
                    {
                        return BadRequest("You are suspended"); // Return a bad request response with an error message
                    }
                    await _context.Users
                     .Include(u => u.Picture) // Include the Picture navigation property
                     .FirstOrDefaultAsync(u => u.Id == currentUser.Id); // Filter by id

                    // Validate the post model using data annotations and custom logic
                    if (ModelState.IsValid && !string.IsNullOrEmpty(postModel.Content))
                    {
                        // Set the post properties that are not provided by the user input
                        var post = new Post
                        {
                            Content = postModel.Content,
                            Id = 0, // The id will be generated by the database automatically
                            UserId = currentUser.Id, // The user is the current user
                            Username = currentUser.UserName!, // The username is the current user's username
                            ProfilePicture = currentUser.Picture?.Url ?? "assets/media/default.png", // The profile picture is the current user's profile picture
                            User = currentUser,
                            Date = DateTime.Now, // The date is the current date and time
                            Likes = new List<Like>(), // The liked by list is initially empty
                            Comments = new List<Comment>(), // The comments list is initially empty
                            Pictures = new List<Picture>(),
                            Videos = new List<Video>()
                        };

                        if (postModel.PictureFiles != null)
                        {
                            foreach (var pictureFile in postModel.PictureFiles)
                            {
                                var pictureUrl = await SaveFile(pictureFile);
                                if (pictureUrl != null)
                                {
                                    post.Pictures.Add(new Picture { Id = 0, Url = pictureUrl, User = currentUser, UserId = currentUser.Id });
                                }
                                else
                                {
                                    return BadRequest("Failed to save picture file"); // Return a bad request response with an error message
                                }
                            }

                        }
                        // Save the uploaded pictures
                        
                        // if VideoFiles is not null
                        
                        if (postModel.VideoFiles != null)
                        {
                            foreach (var videoFile in postModel.VideoFiles)
                            {
                                var videoUrl = await SaveFile(videoFile);
                                if (videoUrl != null)
                                {
                                    post.Videos.Add(new Video { Id = 0, Url = videoUrl, User = currentUser, UserId = currentUser.Id });
                                }
                                else
                                {
                                    return BadRequest("Failed to save video file"); // Return a bad request response with an error message
                                }
                            }

                        }
                        // Save the uploaded videos

                        // Add the post to the database context and save changes
                        _context.Posts.Add(post);
                        await _context.SaveChangesAsync();

                        // Return a created at action response with the post as the data and a location header pointing to the index action
                        // set status code to 201
                        return CreatedAtAction(nameof(Index), post);
                    }
                }

                // If the model is not valid, return a bad request response with the model state errors as the data 
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                
                _logger.LogError(ex, "An error occurred while creating a post.");
                return StatusCode(500, ex.Message); // Return an internal server error response with the error message
            }
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
            
            var strippedFilePath = Path.Combine( "assets", "media", file.FileName);


            // Return the file path
            return strippedFilePath;
        }

        
        // The like action allows the current user to like or unlike a post by its id
        [Authorize] // Require authentication
        [HttpPost("{PostId}/like")] // Only respond to POST requests with a PostId parameter in the route 
        public async Task<IActionResult> Like(int PostId)
        {
            try
            {
                // Get the current user from the user manager
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                // Get the post by its id from the database context
                var post = await _context.Posts.FindAsync(PostId);

                if (currentUser == null || _context == null) { return BadRequest(); }

                // Check if the user is suspended
                if (currentUser.Suspended)
                {
                    return BadRequest("You are suspended"); // Return a bad request response with an error message 
                }


                // Check if the post exists
                if (post == null)
                {
                    return NotFound(); // Return a 404 not found response 
                }

                // Load the related data explicitly
                await _context.Entry(post).Collection(p => p.Likes).LoadAsync();

                if (post.Likes == null)
                {
                    return NotFound(); // Return a 404 not found response 
                }

                // Check if the current user has already liked the post 
                if (post.Likes.Any(l => l.UserId == currentUser.Id))
                {
                    // If yes, remove the current user from the liked by list 
                    post.Likes.Remove(post.Likes.FirstOrDefault(l => l.UserId == currentUser.Id));
                }
                else
                {
                    // If no, add the current user to the liked by list 
                    post.Likes.Add(new Like { UserId = currentUser.Id, PostId = post.Id, DateTime = DateTime.Now });
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
                // Get the current user from the user manager
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                

                if (currentUser == null || _context == null) { return BadRequest(); }

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
                // Get the current user from the user manager
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (currentUser == null || _context == null) { return BadRequest(); }

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
                await _context.Entry(post).Collection(p => p.Likes).LoadAsync();

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
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (currentUser == null || _context == null) { return BadRequest(); }

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
                await _context.Entry(post).Collection(p => p.Likes).LoadAsync();

                // Check if the current user is the owner or admin or moderator of the post
                if (post.UserId == currentUser.Id || currentUser.IsAdmin || currentUser.IsModerator)
                {
                    post.Comments ??= new();
                    post.Likes ??= new();
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
                if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
                {
                    return BadRequest();
                }

                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (currentUser == null || _context == null)
                {
                    return BadRequest();
                }

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
                    await _context.Entry(originalPost).Collection(p => p.Likes).LoadAsync();

                    // Check if the current user is the owner or admin or moderator of the original post
                    if (originalPost.UserId == currentUser.Id || currentUser.IsAdmin || currentUser.IsModerator)
                    {
                        // If yes, update the original post properties that can be edited by the user input
                        originalPost.Content = postModel.Content;

                        // Update the pictures
                        originalPost.Pictures = new List<Picture>();
                        foreach (var pictureFile in postModel.PictureFiles)
                        {
                            // Save the picture file to the server and get the URL
                            var pictureUrl = await SaveFile(pictureFile);

                            // Create a new Picture object and add it to the post's Pictures collection
                            originalPost.Pictures.Add(new Picture { Url = pictureUrl, User = currentUser, UserId = currentUser.Id });
                        }

                        // Update the videos
                        originalPost.Videos = new List<Video>();
                        foreach (var videoFile in postModel.VideoFiles)
                        {
                            // Save the video file to the server and get the URL
                            var videoUrl = await SaveFile(videoFile);

                            // Create a new Video object and add it to the post's Videos collection
                            originalPost.Videos.Add(new Video { Url = videoUrl, User = currentUser, UserId = currentUser.Id });
                        }

                        // Save changes to the database context and reload the original post to reflect any changes made by triggers or computed columns in the database
                        _context.Posts.Update(originalPost);
                        await _context.SaveChangesAsync();
                        await _context.Entry(originalPost).ReloadAsync();

                        // Return a no content response indicating success
                        return NoContent();
                    }

                    // If no, return a 403 forbidden response
                    return Forbid();
                }

                // If the model is not valid, return a bad request response with model state errors as data
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return StatusCode(500, ex.Message); // Return internal server error response with an error message
            }
        }

    }
}