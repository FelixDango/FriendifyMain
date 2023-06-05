using FriendifyMain.Models;
using System.ComponentModel.DataAnnotations;

namespace FriendifyMain.ViewModels
{
    public class UpdateViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters or digits.")]
        public string Username { get; set; }
        [MinLength(1)]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MinLength(1)]
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [MinLength(6)]
        [MaxLength(100)]
        public string Email { get; set; }
        public User.SexEnum Sex { get; set; }
        public User.StatusEnum Status { get; set; }
        [MinLength(1)]
        [MaxLength(200)]
        public string Biography { get; set; }
        [MaxLength(500)]
        public string PictureUrl { get; set; }
    }
}