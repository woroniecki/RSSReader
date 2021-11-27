using AutoMapper;
using DataLayer.Models;
using Dtos.Posts;
using NUnit.Framework;
using ServiceLayer._CQRS.PostQueries;
using System;
using System.Threading.Tasks;

namespace Tests.TechnologyTests
{
    [TestFixture]
    class AutoMapperTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task Test_merging_two_object()
        {
            //ARRANGE
            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserPostData, UserPostDataResponseDto>();
                cfg.CreateMap<Post, PostResponseDto>();
                cfg.CreateMap<PostAndUserDataSelection, PostResponseDto>()
                    .ConstructUsing((src, ctx) => ctx.Mapper.Map<PostResponseDto>(src.Post))
                    .AfterMap((src, dest, context) => {
                        dest.UserData = src.UserPostData == null
                            ? new UserPostDataResponseDto()
                            : context.Mapper.Map<UserPostData, UserPostDataResponseDto>(src.UserPostData);
                    });
            });
            var mapper = configuration.CreateMapper();

            var post_and_data = new PostAndUserDataSelection()
            {
                Post = new Post()
                {
                    Id = 1,
                    Url = "url",
                    Name = "name",
                    Author = "author",
                    ImageUrl = "imageurl",
                    Summary = "summary",
                    Content = "content",
                    PublishDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    FavouriteAmount = 1,
                    BlogId = -1,
                    Blog = null,
                },
                UserPostData = new UserPostData()
                {
                    Id = 2,
                    FirstDateOpen = DateTime.Now,
                    LastDateOpen = DateTime.Now,
                    Readed = true,
                    Favourite = false,
                    // <-- RELATIONS -->
                    PostId = 1,
                    Post = null,
                    User = null,
                    SubscriptionId = 1,
                    Subscription = null,
                }
            };

            //ACT
            var result = mapper.Map<PostAndUserDataSelection, PostResponseDto>(post_and_data);

            //ASSERT
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("name"));
            Assert.IsTrue(result.UserData.Readed);
            Assert.IsFalse(result.UserData.Favourite);
        }
    }
}
