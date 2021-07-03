﻿using AutoMapper;
using Microsoft.Toolkit.Parsers.Rss;

using Dtos.Auth;
using DataLayer.Models;
using Dtos.Groups;
using Dtos.Subscriptions;
using Dtos.Blogs;
using Dtos.Posts;

namespace RSSReader.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApiUser, UserResponseDto>();
            CreateMap<Group, GroupResponseDto>();
            CreateMap<AddGroupRequestDto, Group>();
            CreateMap<Blog, BlogResponseDto>();

            CreateMap<Subscription, UserBlogDataDto>()
                .ForMember(dest => dest.SubId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Subscription, BlogResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Blog.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Blog.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Blog.Description))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Blog.Url))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Blog.ImageUrl))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(src => src));

            CreateMap<Subscription, SubscriptionResponseDto>()
                .ForMember(
                    dest => dest.GroupId,
                    opt => opt.MapFrom(src => src.Group != null ? src.Group.Id : (int?)null)
                    );
            CreateMap<Post, PostResponseDto>();
            CreateMap<RssSchema, Post>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.FeedUrl))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate));
        }
    }
}
