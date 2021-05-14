using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using DataLayer.Models;
using DbAccess.Core;
using Dtos.Groups;
using LogicLayer.Groups;
using ServiceLayer._Commons;

namespace ServiceLayer.GroupServices
{
    public class GroupListService : IGroupListService
    {
        private GetGroupsListAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public GroupListService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GroupResponseDto>> GetList(string userId)
        {
            _action = new GetGroupsListAction(_unitOfWork);

            var result_list = await _action.ActionAsync(userId);

            if (_action.HasErrors)
                return null;

            var returned_list = _mapper.Map<IEnumerable<Group>, IEnumerable<GroupResponseDto>>(result_list);

            return returned_list;
        }
    }

    public interface IGroupListService : IValidatedService
    {
        Task<IEnumerable<GroupResponseDto>> GetList(string userId);
    }
}
