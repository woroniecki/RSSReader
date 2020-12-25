﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSSReader.Dtos;
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
    public class BlogSubscriptionsController : Controller
    {
        private UserManager<IdentityUser> _userManager;

        public BlogSubscriptionsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return Unauthorized();
            
            return Ok(new { data = user });
        }

        [HttpGet]
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
