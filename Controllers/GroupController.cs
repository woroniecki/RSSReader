using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;

using RSSReader.Models;
using RSSReader.Data.Repositories;
using static RSSReader.Data.Repositories.UserRepository;
using static RSSReader.Data.Repositories.GroupRepository;
using static RSSReader.Data.Response;
using RSSReader.Helpers;
using RSSReader.Dtos;
using System.Collections.Generic;
using AutoMapper;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GroupController : Controller
    {
        private IUserRepository _userRepo;
        private IGroupRepository _groupRepo;
        private IMapper _mapper;

        public GroupController(
            IUserRepository userRepo,
            IGroupRepository groupRepo,
            IMapper mapper
            )
        {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<ApiResponse> GetList()
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            var all_groups = await _groupRepo.GetAll(BY_USER(user));

            IEnumerable<GroupForReturnDto> groups_dtos =
                _mapper.Map<IEnumerable<Group>, IEnumerable<GroupForReturnDto>>(all_groups);

            return new ApiResponse(MsgSucceed, groups_dtos, Status200OK);
        }

        [HttpPost("add")]
        public async Task<ApiResponse> Add([FromBody]GroupAddDto data)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            if (string.IsNullOrWhiteSpace(data.Name))
                return ErrBadRequest;

            Group group = _mapper.Map<GroupAddDto, Group>(data);
            group.User = user;

            if (!await _groupRepo.AddAsync(group))
                return ErrRequestFailed;

            GroupForReturnDto group_return_dto = _mapper.Map<Group, GroupForReturnDto>(group);

            return new ApiResponse(MsgSucceed, group_return_dto, Status201Created);
        }

        [HttpDelete("remove/{id}")]
        public async Task<ApiResponse> Remove(int id)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            Group group = await _groupRepo.GetByID(id);

            if (group == null)
                return ErrEntityNotExists;

            if (!await _groupRepo.Remove(group))
                return ErrRequestFailed;

            return new ApiResponse(MsgSucceed, null, Status204NoContent);
        }
    }
}
