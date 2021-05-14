using AutoMapper;
using DataLayer.Models;
using Dtos.Auth;
using RSSReader.Helpers;

namespace Tests.Helpers
{
    public static class MapperHelper
    {
        public static IMapper GetNewInstance()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            return mapper.CreateMapper();
        }
    }
}
