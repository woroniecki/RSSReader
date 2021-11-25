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

        public void Disable(string disabledBy)
        {
            if (UserId != disabledBy)
                throw new Exception("Unauthorized.");

            if (!Active)
                throw new Exception("Subscription is already disabled");

            Active = false;
            LastUnsubscribeDate = DateTime.UtcNow;
        }

        public void SetGroup(string userId, Group group)
        {
            if (UserId != userId)
                throw new Exception("Unauthorized.");

            //-1 is resetting group to null, so it's treaded as none group
            if (group == null)
            {
                GroupId = null;
                Group = null;
            }
            else
            {
                if (group.User.Id != userId)
                    throw new Exception("Unauthorized.");

                GroupId = group.Id;
                Group = group;
            }
        }
    }
}
