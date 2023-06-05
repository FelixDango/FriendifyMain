using System.Text.Json.Serialization;

namespace FriendifyMain.Models
{
    public class Like
    {
        public int UserId { get; set; } // The user who liked the post
        [JsonIgnore]
        public User User { get; set; } // The navigation property to the user
        public int PostId { get; set; } // The post that was liked
        [JsonIgnore]
        public Post Post { get; set; } // The navigation property to the post
    }
}
