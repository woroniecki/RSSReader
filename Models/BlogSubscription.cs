using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class BlogSubscription
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public Blog Blog { get; set; }
        public IdentityUser User { get; set; }
    }
}
