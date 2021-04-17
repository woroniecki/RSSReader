using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Data;
using RSSReader.Models;
using RSSReader.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.Repositories.UserRepository;
using UserPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.ApiUser, bool>>;
using GroupPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Group, bool>>;
using BlogPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Blog, bool>>;
using PostPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.Post, bool>>;
using UserPostDataPred = System.Linq.Expressions.Expression<System.Func<RSSReader.Models.UserPostData, bool>>;
using RSSReader.Dtos;
using RSSReader.Data.Repositories;
using Microsoft.Toolkit.Parsers.Rss;
using AutoWrapper.Wrappers;
using System.IO;
using AutoMapper;
using RSSReader.Helpers;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class GroupControllerTests
    {
        private Mock<IUserRepository> _userRepo;
        private Mock<IGroupRepository> _groupRepo;
        private ApiUser _user;
        private IMapper _mapper;
        GroupController _groupController;

        [SetUp]
        public void SetUp()
        {
            _userRepo = new Mock<IUserRepository>();
            _groupRepo = new Mock<IGroupRepository>();

            _user = new ApiUser()
            {
                Id = "1",
                UserName = "username",
                Email = "user@mail.com"
            };

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });
            _mapper = mapper.CreateMapper();

            _groupController = new GroupController(
                _userRepo.Object,
                _groupRepo.Object,
                _mapper
                );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _groupController.ControllerContext = new ControllerContext();
            _groupController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        #region Mock
        private void Mock_UserRepository_Get(ApiUser returnedUser)
        {
            Expression<Func<IUserRepository, Task<ApiUser>>> expression =
                returnedUser != null ?
                x => x.Get(It.Is<UserPred>(x => x.Compile().Invoke(returnedUser))) :
                x => x.Get(It.IsAny<UserPred>());

            _userRepo.Setup(expression)
            .Returns(Task.FromResult(returnedUser))
            .Verifiable();
        }

        private void Mock_GroupRepository_GetAll(IEnumerable<Group> returnedGroups)
        {
            Expression<Func<IGroupRepository, Task<IEnumerable<Group>>>> expression =
                x => x.GetAll(It.IsAny<GroupPred>());

            _groupRepo.Setup(expression)
            .Returns(Task.FromResult(returnedGroups))
            .Verifiable();
        }
        #endregion

        #region GetList
        [Test]
        public async Task GetList_Ok_ListOfGroups()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);
            List<Group> list_group = new List<Group>();
            for (int i = 0; i < 5; i++)
            {
                list_group.Add(new Group() { 
                    Id = i,
                    Name = i.ToString()
                });
            }

            Mock_GroupRepository_GetAll(list_group);

            //ACT
            var result = await _groupController.GetList();

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status200OK));
            Assert.IsInstanceOf(typeof(IEnumerable<GroupForReturnDto>), result.Result);
            var returned_list = result.Result as IEnumerable<GroupForReturnDto>;
            Assert.That(returned_list.Count, Is.EqualTo(list_group.Count));

            foreach (var group in list_group)
            {
                Assert.That(returned_list.Count(x => x.Id == group.Id), Is.EqualTo(1));
                Assert.That(returned_list.Count(x => x.Name == group.Name), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetList_CantFindUser_Unauthorized()
        {
            //ARRANGE
            Mock_UserRepository_Get(null);

            //ACT
            var result = await _groupController.GetList();

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status401Unauthorized));
        }
        #endregion

        #region Add

        [Test]
        public async Task Add_HappyPath_Ok()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);

            GroupAddDto added_group = new GroupAddDto()
            {
                Name = "name"
            };

            _groupRepo.Setup(x => x.AddAsync(It.IsAny<Group>()))
                .Returns(Task.FromResult(true));

            //ACT
            var result = await _groupController.Add(added_group);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status201Created));
            GroupForReturnDto returned_dto = result.Result as GroupForReturnDto;
            Assert.That(returned_dto.Name, Is.EqualTo(added_group.Name));
        }

        #endregion

        #region Add

        [Test]
        public async Task Delete_HappyPath_Ok()
        {
            //ARRANGE
            Mock_UserRepository_Get(_user);

            GroupAddDto added_group = new GroupAddDto()
            {
                Name = "name"
            };

            _groupRepo.Setup(x => x.GetByID(0))
                .Returns(Task.FromResult(new Group() {Id=0, Name="name", User=_user }));

            _groupRepo.Setup(x => x.Remove(It.IsAny<Group>()))
                .Returns(Task.FromResult(true));

            //ACT
            var result = await _groupController.Remove (0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status204NoContent));
        }

        #endregion
    }
}
