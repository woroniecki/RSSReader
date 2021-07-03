using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Blogs;
using Dtos.Subscriptions;
using LogicLayer._const;
using ServiceLayer._Commons;

namespace ServiceLayer.BlogServices
{
    public class BlogListService : IBlogListService
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public BlogListService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BlogResponseDto>> GetListAsync(string userId)
        {
            var result_list = await _unitOfWork.SubscriptionRepo
                                               .GetListByUserId(userId, RssConsts.POSTS_PER_CALL);

            var returned_list = _mapper.Map<IEnumerable<Subscription>, IEnumerable<BlogResponseDto>>(result_list);

            return returned_list;
        }
    }

    public interface IBlogListService
    {
        Task<IEnumerable<BlogResponseDto>> GetListAsync(string userId);
    }
}
