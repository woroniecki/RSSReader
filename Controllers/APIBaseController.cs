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
        private UserManager<IdentityUser> _userManager;

        public APIBaseController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected virtual async Task<IdentityUser> GetCurrentUser()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await _userManager.FindByIdAsync(id);
        }
    }
}
