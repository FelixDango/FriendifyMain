using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FriendifyMain.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User,Id")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } // navigation property to User
        [ForeignKey("User,Id")]
        public int ReceiverId { get; set; }
        [JsonIgnore]
        public User Receiver { get; set; } // navigation property to User
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<Video> Videos { get; set; }
    }
}