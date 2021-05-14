using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Groups;

using LogicLayer.Groups;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.GroupServices
{
    public class GroupAddService : IGroupAddService
    {
        private AddGroupAction _action;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public GroupAddService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GroupResponseDto> AddNewGroup(AddGroupRequestDto dataIn, string userId)
        {
            _action = new AddGroupAction(userId, _mapper, _unitOfWork);

            var runner = new RunnerWriteDbAsync<AddGroupRequestDto, Group>(
                _action,
                _unitOfWork.Context
                );

            var result = await runner.RunActionAsync(dataIn);

            if (runner.HasErrors)
                return null;

            var returned_dto = _mapper.Map<GroupResponseDto>(result);

            return returned_dto;
        }
    }

    public interface IGroupAddService : IValidatedService
    {
        Task<GroupResponseDto> AddNewGroup(AddGroupRequestDto dataIn, string userId);
    }
}
