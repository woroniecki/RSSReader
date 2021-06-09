using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Jobs
{
    public class UpdateBlogsJobResponse
    {
        public List<string> Failed { get; set; }
        public List<string> Succeeded { get; set; }
        public List<string> NoUpdate { get; set; }
    }
}
