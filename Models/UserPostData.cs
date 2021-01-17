using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Models
{
    public class UserPostData
    {
        public UserPostData() { }
        public UserPostData(Post post, ApiUser user) {
            this.Post = post;
            this.User = user;
            FirstDateOpen = DateTime.UtcNow;
            LastDateOpen = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public DateTime FirstDateOpen { get; set; }
        public DateTime LastDateOpen { get; set; }
        public bool Readed { get; set; }
        public bool Favourite { get; set; }
        public Post Post { get; set; }
        public ApiUser User { get; set; }
    }
}
