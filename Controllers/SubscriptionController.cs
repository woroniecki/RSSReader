﻿using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Data.Repositories;
using RSSReader.Dtos;
using RSSReader.Helpers;
using RSSReader.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.Repositories.UserRepository;
using static RSSReader.Data.Repositories.BlogRepository;
using static RSSReader.Data.Repositories.SubscriptionRepository;
using RSSReader.Data;
using System.Collections.Generic;
using Microsoft.Toolkit.Parsers.Rss;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly ISubscriptionRepository _subRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IFeedService _feedService;
        private readonly IHttpService _httpService;

        public SubscriptionController(
            IUserRepository userRepository,
            IReaderRepository readerRepository,
            ISubscriptionRepository subRepository,
            IBlogRepository blogRepository,
            IFeedService feedService,
            IHttpService httpService)
        {
            _userRepository = userRepository;
            _readerRepository = readerRepository;
            _subRepository = subRepository;
            _blogRepository = blogRepository;
            _feedService = feedService;
            _httpService = httpService;
        }

        [HttpGet("list")]
        public async Task<ApiResponse> GetList()
        {
            ApiUser user = await _userRepository
                .GetWithSubscriptions(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            //TODO async and move to repo, needs tests
            var subs = user.Subscriptions.Where(x => x.Active);

            return new ApiResponse(MsgSucceed, subs, Status200OK);
        }

        [HttpPost("subscribe")]
        public async Task<ApiResponse> Subscribe(SubscriptionForAddDto subscriptionForAddDto)
        {
            ApiUser user = await _userRepository
                .Get(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            Blog blog = await _blogRepository.Get(BY_BLOGURL(subscriptionForAddDto.BlogUrl));
            Subscription subscription = null;

            if (blog == null)
            {
                string feed_content = await _httpService.GetStringContent(subscriptionForAddDto.BlogUrl);
                if(string.IsNullOrEmpty(feed_content))
                    return ErrInvalidFeedUrl;

                IEnumerable<RssSchema> feed = _feedService.ParseFeed(feed_content);
                if(feed == null)
                    return ErrNoContentUnderFeedUrl;

                blog = _feedService.CreateBlogObject(
                    subscriptionForAddDto.BlogUrl,
                    feed_content,
                    feed);

                if (!await _blogRepository.AddAsync(blog))
                    return ErrRequestFailed;
            }
            else
            {
                subscription = await _subRepository.Get(BY_USERANDBLOG(user, blog));
            }

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
                subscription.LastSubscribeDate = DateTime.UtcNow;

                if (!await _readerRepository.SaveAllAsync())
                    return ErrEntityNotExists;

                return new ApiResponse(MsgSucceed, subscription, Status200OK);
            }

            return ErrSubAlreadyEnabled;
        }

        [HttpPut("{id}/unsubscribe")]
        public async Task<ApiResponse> Unsubscribe(int id)
        {
            ApiUser user = await _userRepository
                .GetWithSubscriptions(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            var sub = user.Subscriptions.Where(x => x.Id == id).FirstOrDefault();

            if (sub == null)
                return ErrEntityNotExists;//Never should happend

            if (!sub.Active)
                return ErrSubAlreadyDisabled;

            sub.Active = false;
            sub.LastUnsubscribeDate = DateTime.UtcNow;

            if(!await _readerRepository.SaveAllAsync())
                return ErrRequestFailed;

            return new ApiResponse(MsgSucceed, sub, Status200OK);
        }
    }
}
