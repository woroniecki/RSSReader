using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Groups;

using LogicLayer._GenericActions;

namespace LogicLayer.Groups
{
    public class AddGroupAction :
        ActionErrors,
        IActionAsync<AddGroupRequestDto, Group>
    {
        private string _requestingUserId;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public AddGroupAction(string requestingUserId, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _requestingUserId = requestingUserId;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Group> ActionAsync(AddGroupRequestDto dto)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_requestingUserId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }

            Group same_name_group = await _unitOfWork.GroupRepo.GetUserIdAndByName(user.Id, dto.Name);

            if(same_name_group != null)
            {
                AddError("Group with the same name already exists.");
                return null;
            }

            Group group = _mapper.Map<AddGroupRequestDto, Group>(dto);
            group.User = user;
            _unitOfWork.GroupRepo.AddNoSave(group);

            return group;
        }
    }
}
