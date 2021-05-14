using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ApiUser User { get; set; }
    }
}
