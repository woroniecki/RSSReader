using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class ApiUser : IdentityUser
    {
        [JsonIgnore]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
