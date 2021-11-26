using System;

namespace DataLayer.Models
{
    public class UserPostData
    {
        public UserPostData() { }
        public UserPostData(Post post, ApiUser user, Subscription sub) {
            this.Post = post;
            this.User = user;
            this.Subscription = sub;
            Readed = true;
            Favourite = false;
            FirstDateOpen = DateTime.UtcNow;
            LastDateOpen = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public DateTime FirstDateOpen { get; set; }
        public DateTime LastDateOpen { get; set; }
        public bool Readed { get; set; }
        public bool Favourite { get; set; }
        // <-- RELATIONS -->
        public int PostId { get; set; }
        public Post Post { get; set; }
        public ApiUser User { get; set; }
        public int? SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        public void SetReaded(bool value)
        {
            if (value == true)
                LastDateOpen = DateTime.UtcNow;

            Readed = value;
        }

        public void SetFavourite(bool value)
        {
            Post.FavouriteAmount += value ? 1 : -1;
            Post.FavouriteAmount = Math.Max(0, Post.FavouriteAmount);
            Favourite = value;
        }
    }
}
