﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Admin
{
    public class SetUserRoleRequestDto
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
