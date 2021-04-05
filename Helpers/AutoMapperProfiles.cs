using AutoMapper;
using Microsoft.Toolkit.Parsers.Rss;
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
            CreateMap<RssSchema, Post>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.FeedUrl))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate));
            CreateMap<Post, PostDataForReturnDto>();
        }
    }
}
