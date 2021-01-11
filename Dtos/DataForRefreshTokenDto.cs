using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class DataForRefreshTokenDto
    {
        public string RefreshToken { get; set; }
        public string AuthToken { get; set; }
    }
}
