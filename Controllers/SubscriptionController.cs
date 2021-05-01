using AutoWrapper.Wrappers;
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
using AutoMapper;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        private readonly IUnitOfWork _UOW;
        private readonly IFeedService _feedService;
        private readonly IHttpService _httpService;
        private readonly IMapper _mapper;

        public SubscriptionController(
            IUnitOfWork unitOfWork,
            IFeedService feedService,
            IHttpService httpService,
            IMapper mapper)
        {
            _UOW = unitOfWork;
            _feedService = feedService;
            _httpService = httpService;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<ApiResponse> GetList()
        {
            ApiUser user = await _UOW.UserRepo.GetWithSubscriptions(x => x.Id == this.GetCurUserId());
            if (user == null)
                return ErrUnauhtorized;

            //TODO async and move to repo, needs tests
            var subs = user.Subscriptions.Where(x => x.Active);

            IEnumerable<SubscriptionForReturnDto> subs_to_return =
                _mapper.Map<IEnumerable<Subscription>, IEnumerable<SubscriptionForReturnDto>>(subs);

            return new ApiResponse(MsgSucceed, subs_to_return, Status200OK);
        }

        [HttpPost("subscribe")]
        public async Task<ApiResponse> Subscribe(SubscriptionForAddDto subscriptionForAddDto)
        {
            ApiUser user = await _UOW.UserRepo.GetByID(this.GetCurUserId());
            if (user == null)
                return ErrUnauhtorized;

            Blog blog = await _UOW.BlogRepo.GetByUrl(subscriptionForAddDto.BlogUrl);
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

                _UOW.BlogRepo.AddNoSave(blog);
            }
            else
            {
                subscription = await _UOW.SubscriptionRepo.GetByUserAndBlog(user, blog);
            }

            if (subscription == null)
            {
                subscription = new Subscription(user, blog);

                _UOW.SubscriptionRepo.AddNoSave(subscription);

                if (!await _UOW.ReaderRepo.SaveAllAsync())
                    return ErrRequestFailed;

                return new ApiResponse(MsgCreated, subscription, Status201Created);
            }
            else if (!subscription.Active)
            {
                subscription.Active = true;
                subscription.LastSubscribeDate = DateTime.UtcNow;

                if (!await _UOW.ReaderRepo.SaveAllAsync())
                    return ErrEntityNotExists;

                return new ApiResponse(MsgSucceed, subscription, Status200OK);
            }

            return ErrSubAlreadyEnabled;
        }

        [HttpPut("{id}/unsubscribe")]
        public async Task<ApiResponse> Unsubscribe(int id)
        {
            ApiUser user = await _UOW.UserRepo.GetWithSubscriptions(x => x.Id == this.GetCurUserId());
            if (user == null)
                return ErrUnauhtorized;

            var sub = user.Subscriptions.Where(x => x.Id == id).FirstOrDefault();

            if (sub == null)
                return ErrEntityNotExists;//Never should happend

            if (!sub.Active)
                return ErrSubAlreadyDisabled;

            sub.Active = false;
            sub.LastUnsubscribeDate = DateTime.UtcNow;

            if(!await _UOW.ReaderRepo.SaveAllAsync())
                return ErrRequestFailed;

            return new ApiResponse(MsgSucceed, sub, Status200OK);
        }

        [HttpPatch("{subid}/set_group/{groupid}")]
        public async Task<ApiResponse> SetGroup(int subid, int groupid)
        {
            ApiUser user = await _UOW.UserRepo.GetByID(this.GetCurUserId());
            if (user == null)
                return ErrUnauhtorized;

            Subscription sub = await _UOW.SubscriptionRepo.GetByIdWithUserAndGroup(subid);
            if (sub == null)
                return ErrEntityNotExists;

            if (sub.User.Id != user.Id)
                return ErrUnauhtorized;

            Group group = null;

            //If -1 set group to none
            if (groupid != -1)
            {
                group = await _UOW.GroupRepo.GetByID(groupid);
                if (group == null)
                    return ErrEntityNotExists;
            }

            sub.Group = group;

            if (!await _UOW.ReaderRepo.SaveAllAsync())
                return ErrRequestFailed;

            SubscriptionForReturnDto sub_to_return =
                _mapper.Map<Subscription, SubscriptionForReturnDto>(sub);

            return new ApiResponse(MsgUpdated, sub_to_return, Status200OK);
        }
    }
}
