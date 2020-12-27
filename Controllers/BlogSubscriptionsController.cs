using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogSubscriptionsController : APIBaseController
    {
        public BlogSubscriptionsController(UserManager<IdentityUser> userManager)
            : base(userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            return Ok(new { data = user });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSubscribedBlogsList()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogSubscription(BlogSubscriptionForAddDto blogSubscriptionForAddDto)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DisableBlogSubscription()
        {
            return Ok();
        }
    }
}
