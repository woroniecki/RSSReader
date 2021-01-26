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
            CreateMap<RefreshToken, TokenForReturnDto>()
                .ForMember(dest => dest.Expires, opt => 
                opt.MapFrom(src => src.Expires.From1970()));
        }
    }
}
