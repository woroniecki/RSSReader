using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.Controllers
{
    public abstract class APIBaseController : Controller
    {
        private UserManager<ApiUser> _userManager;

        public APIBaseController(UserManager<ApiUser> userManager)
        {
            _userManager = userManager;
        }

        protected virtual async Task<ApiUser> GetCurrentUser()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await _userManager.FindByIdAsync(id);
        }
    }
}
