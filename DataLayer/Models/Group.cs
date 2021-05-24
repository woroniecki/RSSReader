using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // <-- RELATIONS -->
        [JsonIgnore]
        public virtual ApiUser User { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
