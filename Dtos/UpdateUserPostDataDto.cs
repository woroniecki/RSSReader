using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class UpdateUserPostDataDto
    {
        public bool? Readed { get; set; }
        public bool? Favourite { get; set; }
    }
}
