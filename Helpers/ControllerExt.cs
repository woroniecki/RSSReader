using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.Helpers
{
    public static class ControllerExt
    {
        public static string GetCurUserId(this Controller controller)
        {
            return controller.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
