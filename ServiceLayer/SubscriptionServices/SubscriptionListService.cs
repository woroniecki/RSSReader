using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Subscriptions;
using LogicLayer._const;
using ServiceLayer._Commons;

namespace ServiceLayer.SubscriptionServices
{
    public class SubscriptionListService : ISubscriptionListService
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public SubscriptionListService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetListAsync(string userId)
        {
            var result_list = await _unitOfWork.SubscriptionRepo
                                               .GetListByUserId(userId, RssConsts.POSTS_PER_CALL);

            var returned_list = _mapper.Map<IEnumerable<Subscription>, IEnumerable<SubscriptionResponseDto>>(result_list);

            return returned_list;
        }
    }

    public interface ISubscriptionListService
    {
        Task<IEnumerable<SubscriptionResponseDto>> GetListAsync(string userId);
    }
}
