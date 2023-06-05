﻿using System.Text.Json.Serialization;

namespace FriendifyMain.Models
{
    public class Follower
    {
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public User Following { get; set; }
        public int UserId { get; set; }
        public int FollowerId { get; set; }
    }

}
