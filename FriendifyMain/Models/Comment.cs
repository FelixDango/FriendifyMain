using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FriendifyMain.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Post,Id")]
        public int PostId { get; set; }
        public Post Post { get; set; } // navigation property to Post
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
