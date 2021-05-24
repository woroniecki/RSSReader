using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Groups
{
    public class RemoveGroupRequestDto
    {
        public int GroupId { get; set; }
        public bool UnsubscribeSubscriptions { get; set; }
    }
}
