using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dtos.Posts
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }

        /// User Data
        public UserPostDataResponseDto UserData{ get; set; }
    }

    public class UserPostDataResponseDto
    {
        public UserPostDataResponseDto()
        {
            Readed = false;
            Favourite = false;
        }
        public bool Readed { get; set; }
        public bool Favourite { get; set; }
    }
}
