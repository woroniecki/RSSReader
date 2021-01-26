using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class TokenForReturnDto
    {
        public string Token { get; set; }
        public long Expires { get; set; }
    }
}
