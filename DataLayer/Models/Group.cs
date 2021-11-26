using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Group
    {
        public Group() { }
        public Group(Guid guid, string name, ApiUser user) 
        {
            Guid = guid;
            Name = name;
            User = user;
        }

        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }

        // <-- RELATIONS -->
        [JsonIgnore]
        public virtual ApiUser User { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
