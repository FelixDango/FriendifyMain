using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public enum StatusEnum { Single, Relationship, Married}

        [Key]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters or digits.")]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool Suspended { get; set; }
        [Required]
        public SexEnum Sex { get; set; }
        [Required]
        public StatusEnum Status { get; set; }

        public Picture? Picture { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Follower> Follows { get; set; }
        public List<Follower> FollowedBy { get; set; }
        public List<Post> Posts { get; set; }
        public List<AssignedRole> AssignedRoles { get; set; }


        // Moderator fields
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool IsModerator { get; set; }
        public bool IsAdmin { get; set; }
    }
}
