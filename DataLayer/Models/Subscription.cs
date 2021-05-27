using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Subscription
    {
        public Subscription() { }
        public Subscription(string userId, Blog blog)
        {
            Active = true;
            FirstSubscribeDate = DateTime.UtcNow;
            LastSubscribeDate = DateTime.UtcNow;
            LastUnsubscribeDate = DateTime.MinValue;
            BlogId = blog.Id;
            Blog = blog;
            UserId = userId;
            FilterReaded = false;
        }
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public bool FilterReaded { get; set; }

        // <-- RELATIONS -->
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public string UserId { get; set; }
        public ApiUser User { get; private set; }
        public ICollection<UserPostData> UserPostDatas { get; set; }
        // <-- NOT MAPPED -->
        [NotMapped]
        public int? UnreadedCount { get; set; }
    }
}
