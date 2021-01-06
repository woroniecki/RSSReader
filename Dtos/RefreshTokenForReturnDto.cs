using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class RefreshTokenForReturnDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
