using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class ApiUser : IdentityUser
    {
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
