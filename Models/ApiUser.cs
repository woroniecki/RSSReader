using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class ApiUser : IdentityUser
    {
        [JsonIgnore]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
