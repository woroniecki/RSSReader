using AutoWrapper.Wrappers;
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
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

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
        public async Task<ApiResponse> GetList()
        {
            ApiUser user = await GetCurrentUser();

            if (user == null)
                return ErrUnauhtorized;

            var subs = user.Subscriptions.Where(x => x.Active);

            return new ApiResponse(MsgSucceed, subs, Status200OK);
        }

        [HttpPost("subscribe")]
        public async Task<ApiResponse> Subscribe(SubscriptionForAddDto subscriptionForAddDto)
        {
            ApiUser user = await GetCurrentUser();

            if (user == null)
                return ErrUnauhtorized;

            Blog blog = await _blogRepository
                .GetByUrlAsync(subscriptionForAddDto.BlogUrl);

            if (blog == null)
            {
                blog = new Blog()
                {
                    Name = subscriptionForAddDto.BlogUrl,
                    Url = subscriptionForAddDto.BlogUrl
                };

                if (!await _blogRepository.AddAsync(blog))
                    return ErrRequestFailed;
            }

            Subscription subscription = await _subRepository
                .GetByUserAndBlogAsync(user, blog);

            if (subscription == null)
            {
                subscription = new Subscription(user, blog);

                if (!await _subRepository.AddAsync(subscription))
                    return ErrRequestFailed;

                return new ApiResponse(MsgCreated, subscription, Status201Created);
            }
            else if (!subscription.Active)
            {
                subscription.Active = true;
                subscription.LastSubscribeDate = DateTime.Now;

                if (!await _readerRepository.SaveAllAsync())
                    return ErrEntityNotExists;

                return new ApiResponse(MsgSucceed, subscription, Status200OK);
            }

            return ErrSubAlreadyEnabled;
        }

        [HttpPost("{id}/unsubscribe")]
        public async Task<ApiResponse> Unsubscribe(int id)
        {
            ApiUser user = await GetCurrentUser();

            if (user == null)
                return ErrUnauhtorized;

            if (!user.Subscriptions.Any(x => x.Id == id))
                return ErrUnauhtorized;

            var sub = await _subRepository.Get(id);

            if (sub == null)
                return ErrEntityNotExists;//Never should happend

            if (!sub.Active)
                return ErrSubAlreadyDisabled;

            sub.Active = false;
            sub.LastUnsubscribeDate = DateTime.Now;

            if(!await _readerRepository.SaveAllAsync())
                return ErrRequestFailed;

            return new ApiResponse(MsgSucceed, sub, Status200OK);
        }
    }
}
