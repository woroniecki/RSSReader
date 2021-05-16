using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dtos.Blogs;
using Dtos.Groups;

namespace Dtos.Subscriptions
{
    public class SubscriptionResponseDto
    {
        public int Id { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public BlogResponseDto Blog { get; set; }
        public int? UnreadedCount { get; set; }
        public int? GroupId { get; set; }
    }
}
