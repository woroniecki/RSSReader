using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class PostDataForReturnDto
    {
        public PostDataForReturnDto()
        {
            Readed = false;
            Favourite = false;
        }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        
        /// User Data
        public bool Readed { get; set; }
        public bool Favourite { get; set; }
    }
}
