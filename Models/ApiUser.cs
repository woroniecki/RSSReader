using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class ApiUser : IdentityUser
    {
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
