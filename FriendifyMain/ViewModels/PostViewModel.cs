using System.ComponentModel.DataAnnotations;

namespace FriendifyMain.ViewModels
{
    public class PostViewModel
    {
        [Required] // The content is required
        [StringLength(500)] // The content cannot exceed 500 characters
        public string Content { get; set; } // The text of the post
        public List<IFormFile>? PictureFiles { get; set; }
        public List<IFormFile>? VideoFiles { get; set; }
    }
}
