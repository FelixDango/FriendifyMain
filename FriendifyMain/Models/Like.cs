namespace FriendifyMain.Models
{
    public class Like
    {
        public int UserId { get; set; } // The user who liked the post
        public User User { get; set; } // The navigation property to the user
        public int PostId { get; set; } // The post that was liked
        public Post Post { get; set; } // The navigation property to the post
    }
}
