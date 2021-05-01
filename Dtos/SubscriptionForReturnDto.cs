using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class SubscriptionForReturnDto
    {
        public int Id { get; set; }
        public DateTime FirstSubscribeDate { get; set; }
        public DateTime LastSubscribeDate { get; set; }
        public DateTime LastUnsubscribeDate { get; set; }
        public virtual Blog Blog { get; set; }
        public virtual int GroupId { get; set; }
    }
}
