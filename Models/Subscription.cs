﻿using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class Subscription
    {
        public Subscription() { }
        public Subscription(ApiUser user, Blog blog)
        {
            Active = true;
            FirstSubscribeDate = DateTime.Now;
            LastSubscribeDate = DateTime.Now;
            LastUnsubscribeDate = DateTime.MinValue;
            Blog = blog;
            User = user;
        }
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public virtual Blog Blog { get; set; }

        [JsonIgnore]
        public virtual ApiUser User { get; set; }
    }
}
