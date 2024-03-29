﻿using System.ComponentModel.DataAnnotations;

namespace FriendifyMain.ViewModels
{
    public class MessageViewModel
    {
        [Required] // The content is required
        [StringLength(500)] // The content cannot exceed 500 characters
        public string Content { get; set; } // The text of the post

        [Required] // The Receiver is required
        [StringLength(50)] // The username cannot exceed 50 characters
        public string ReceiverUsername { get; set; } //Receiver username

        public List<string>? Pictures { get; set; } // The names of the pictures to be uploaded with the post

        public List<string>? Videos { get; set; } // The names of the videos to be uploaded with the post
    }
}
