using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendifyMain.Models
{
    /// <summary>
    /// Data model for user data 
    /// </summary>
    public class User
    {
        // Help types
        public enum SexEnum { Male, Female, Other }
        public enum StatusEnum { Single, Relationship, Married}


        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public bool Suspended { get; set; }
        public SexEnum Sex { get; set; }
        public StatusEnum Status { get; set; }

        public Picture? Picture { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Follower> Follows { get; set; }
        public List<Follower> FollowedBy { get; set; }
        public List<Post> Posts { get; set; }

        // Moderator fields
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool IsModerator { get; set; }
        public bool IsAdmin { get; set; }
    }
}
