using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FriendifyMain.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string ProfilePicture { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        [JsonInclude]
        public List<Like> Likes { get; set; }
        [JsonInclude]
        public List<Comment> Comments { get; set; }
        [JsonInclude]
        public List<Picture> Pictures { get; set; }
        [JsonInclude]
        public List<Video> Videos { get; set; }
    }

}
