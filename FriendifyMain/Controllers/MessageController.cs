﻿using FriendifyMain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;

namespace FriendifyMain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        // Inject the database context and the user manager
        private readonly FriendifyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper; // Declare the _mapper variable

        public MessageController(FriendifyContext context, UserManager<User> userManager,
                                 IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // The get action allows an authenticated user to get their own messages or an admin to get any messages by user id
        [HttpGet("{id}")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(List<Message>), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Get(int id) // Indicate that the id is bound from route data
        {
            // Get the current user from the user manager
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is an admin or requesting their own messages
            if (currentUser != null && (currentUser.IsAdmin || currentUser.Id == id))
            {
                // Get the messages by user id from the database context
                var messages = await _context.Messages.Where(m => m.UserId == id).ToListAsync();

                // Return a 200 OK response with the messages data
                return Ok(messages);
            }

            // If not, return a 403 forbidden response
            return Forbid();
        }

        // The create action allows an authenticated user to create a new message with optional pictures and videos
        [HttpPost]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(Message), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(ModelStateDictionary), 400)] // Specify possible response type and status code
        public async Task<IActionResult> Create([FromBody] MessageViewModel messageModel) // Indicate that the model is bound from form data
        {
            try
            {

                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Get the receiver from user manager by username
                var receiverUser = await _userManager.FindByNameAsync(messageModel.ReceiverUsername);


                // Validate the message model using data annotations and custom logic
                if (currentUser != null && receiverUser != null && ModelState.IsValid && !string.IsNullOrEmpty(messageModel.Content))
                {
                    // Set the message properties that are not provided by the user input
                    var message = new Message
                    {
                        Content = messageModel.Content,
                        Id = 0, // The id will be generated by the database automatically
                        UserId = currentUser.Id, // The sender is the current user
                        ReceiverId = receiverUser.Id, // Assign the ID of the receiver user
                        Date = DateTime.Now, // The date is the current date and time
                        Pictures = new List<Picture>(),
                        Videos = new List<Video>()
                    };
                    messageModel.Pictures.ForEach(e => message.Pictures.Add(new Picture { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));
                    messageModel.Videos.ForEach(e => message.Videos.Add(new Video { Id = 0, Url = e, User = currentUser, UserId = currentUser.Id }));


                    // Add the message to the database context and save changes
                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    // Return a 200 OK response with the message data
                    return Ok(message);
                }

                // If the model is not valid, return a 400 Bad Request response with validation errors
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }
        }

        // The delete action allows an authenticated user to delete a message by its id if they are the owner or a moderator
        [HttpDelete("{id}/delete")]
        [Authorize] // Require authentication
        [ProducesResponseType(200)] // Specify possible response status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Delete(int id) // Indicate that the id is bound from route data
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);

                // Get the message by its id from the database context
                var message = await _context.Messages.FindAsync(id);

                // Check if the message exists
                if (message == null)
                {
                    return NotFound("Message not found."); //Return a 404 not found response with an error message
                }

                // Check if the current user is the owner or a moderator of the message
                if (currentUser != null && (message.UserId == currentUser.Id || currentUser.IsModerator || currentUser.IsAdmin))
                {
                    // If yes, remove the message from the database context and save changes
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();

                    // Return a 200 OK response
                    return Ok();
                }

                // If no, return a 403 forbidden response
                return Forbid();
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }
        }

        // The edit action allows the owner or admin or moderator to edit a message by its id
        [HttpPut("{id}/edit")]
        [Authorize] // Require authentication
        [ProducesResponseType(typeof(Message), 200)] // Specify possible response type and status code
        [ProducesResponseType(typeof(ModelStateDictionary), 400)] // Specify possible response type and status code
        [ProducesResponseType(typeof(string), 404)] // Specify possible response type and status code
        public async Task<IActionResult> Edit(int id, [FromBody] Message model) // Indicate that the id is bound from route data and the model is bound from form data
        {
            try
            {
                // Get the current user from the user manager
                var currentUser = await _userManager.GetUserAsync(User);


                // Validate the message model using data annotations and custom logic
                if (ModelState.IsValid && !string.IsNullOrEmpty(model.Content))
                {
                    // Get the original message by its id from the database context
                    var originalMessage = await _context.Messages.FindAsync(id);

                    // Check if the original message exists and matches with the given message id
                    if (originalMessage == null || originalMessage.Id != model.Id)
                    {
                        return NotFound("Message not found."); // Return a 404 not found response with an error message
                    }

                    // Check if the current user is the owner or admin or moderator of the original message
                    if (originalMessage.UserId == currentUser.Id || currentUser.IsAdmin || currentUser.IsModerator)
                    {
                        // If yes, update the original message properties that can be edited by the user input
                        originalMessage.Content = model.Content;
                        originalMessage.Pictures = model.Pictures;
                        originalMessage.Videos = model.Videos;

                        // Save the changes to the database context and reload the original message to reflect any changes made by triggers or computed columns in the database 
                        _context.Messages.Update(originalMessage);
                        await _context.SaveChangesAsync();
                        await _context.Entry(originalMessage).ReloadAsync();

                        // Return a 200 OK response with the updated message data
                        return Ok(originalMessage);
                    }

                    // If no, return a 403 forbidden response
                    return Forbid();
                }

                // If the model is not valid, return a 400 Bad Request response with validation errors
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle any possible exceptions
                return View("Error", ex.Message); // Return a view that shows the error message
            }
        }
    }
}
