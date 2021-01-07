using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class TokenForReturnDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
