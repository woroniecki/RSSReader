using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RSSReader.Data;
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
    public class SubscriptionController : APIBaseController
    {
        private readonly IReaderRepository _readerRepository;
        private readonly ISubRepository _subRepository;
        private readonly IBlogRepository _blogRepository;

        public SubscriptionController(
            UserManager<ApiUser> userManager,
            IReaderRepository readerRepository,
            ISubRepository subRepository,
            IBlogRepository blogRepository)
            : base(userManager)
        {
            this._readerRepository = readerRepository;
            this._subRepository = subRepository;
            this._blogRepository = blogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApiUser user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            return Ok(new { data = user });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            ApiUser user = await GetCurrentUser();

            if (user == null)
                return Unauthorized("Auth failed");

            var subs = user.Subscriptions;

            return Ok(subs);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Subscribe(SubscriptionForAddDto subscriptionForAddDto)
        {
            ApiUser user = await GetCurrentUser();
            
            if (user == null)
                return Unauthorized("Auth failed");

            Blog blog = await _blogRepository
                .GetByUrlAsync(subscriptionForAddDto.BlogUrl);

            if (blog == null)
            {
                blog = new Blog()
                {
                    Name = subscriptionForAddDto.BlogUrl,
                    Url = subscriptionForAddDto.BlogUrl
                };

                if(!await _blogRepository.AddAsync(blog))
                    return BadRequest("Internal server error");
            }

            Subscription subscription = await _subRepository
                .GetByUserAndBlogAsync(user, blog);

            if (subscription == null)
            {
                subscription = new Subscription(user, blog);

                if(!await _subRepository.AddAsync(subscription))
                    return BadRequest("Internal server error");

                return Created("route to set", subscription);
            }
            else if(!subscription.Active)
            {
                subscription.Active = true;
                if(!await _readerRepository.SaveAllAsync())
                    return BadRequest("Internal server error");

                return Ok(subscription);
            }

            return Ok(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe()
        {
            return Ok();
        }
    }
}
