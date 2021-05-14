using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Subscriptions
{
    public class SubscribeRequestDto
    {
        [Required]
        public string BlogUrl { get; set; }
    }
}
