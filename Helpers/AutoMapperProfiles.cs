using AutoMapper;
using RSSReader.Dtos;
using RSSReader.Models;

namespace RSSReader.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApiUser, UserForReturnDto>();
            CreateMap<RefreshToken, RefreshTokenForReturnDto>();
        }
    }
}
