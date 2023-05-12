namespace FriendifyMain.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public List<User> LikedBy { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<Video> Videos { get; set; }
    }

}
