using DataLayer.Models;

namespace ServiceLayer._CQRS.PostQueries
{
    public class PostAndUserDataSelection
    {
        public Post Post { get; set; }
        public UserPostData UserPostData { get; set; }
    }
}
