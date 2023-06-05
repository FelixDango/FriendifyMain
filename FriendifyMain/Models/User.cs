using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FriendifyMain.Models
{
    /// <summary>
    /// Data model for user data 
    /// </summary>
    public class User : IdentityUser<int>
    {
        // Help types
        public enum SexEnum { Male, Female, Other }
        public enum StatusEnum { Single, Relationship, Married }

        // Remove these properties and use the inherited ones from IdentityUser
        // [Required]
        // [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters or digits.")]
        // public string Username { get; set; }
        // [Required]
        // public string Password { get; set; }
        // [Required]
        // public string Email { get; set; }


        [Key]
        public override int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public SexEnum Sex { get; set; }
        [Required]
        public StatusEnum Status { get; set; }

        
        public string Biography { get; set; }
        public bool Suspended { get; set; }
        public Picture? Picture { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        [JsonIgnore]
        public List<User> Follows { get; set; }
        [JsonInclude]
        public List<int> FollowsIds { get; set; }
        [JsonIgnore]
        public List<User> FollowedBy { get; set; }
        [JsonInclude]
        public List<int> FollowedByIds { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<AssignedRole> AssignedRoles { get; set; }
        public List<Picture> Images { get; set; }
        public List<Video> Videos { get; set; }
        public List<Message> Messages { get; set; }
        public List<Like> Likes { get; set; }



        // Moderator fields
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool IsModerator { get; set; }
        public bool IsAdmin { get; set; }
    }
}