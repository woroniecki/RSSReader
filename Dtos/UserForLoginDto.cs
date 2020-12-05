using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Dtos
{
    public class UserForLoginDto
    {
        public string UsernameOrEmail { get; set; }

        public string Password { get; set; }
    }
}
