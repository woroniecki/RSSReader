using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RSSReader.Controllers;
using RSSReader.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using RSSReader.Dtos;
using AutoMapper;
using RSSReader.Helpers;
using RSSReader.UnitTests.Wrappers.Repositories;

namespace RSSReader.UnitTests
{
    [TestFixture]
    class GroupControllerTests
    {
        private MockUOW _mockUOW;
        private ApiUser _user;
        private IMapper _mapper;
        private GroupController _groupController;

        [SetUp]
        public void SetUp()
        {
            _mockUOW = new MockUOW();

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
                _mockUOW.Object,
                _mapper
                );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.NameIdentifier, _user.Id)
                               }, "TestAuthentication"));
            _groupController.ControllerContext = new ControllerContext();
            _groupController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        #region GetList
        [Test]
        public async Task GetList_Ok_ListOfGroups()
        {
            //ARRANGE
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);

            List<Group> list_group = new List<Group>();
            for (int i = 0; i < 5; i++)
            {
                list_group.Add(new Group() { 
                    Id = i,
                    Name = i.ToString()
                });
            }

            _mockUOW.GroupRepo.SetGetListByUser(_user, list_group);

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
            _mockUOW.UserRepo.SetGetByID(_user.Id, null);

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
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);

            GroupAddDto added_group = new GroupAddDto()
            {
                Name = "name"
            };

            _mockUOW.GroupRepo.SetAdd(It.IsAny<Group>(), true);

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
            _mockUOW.UserRepo.SetGetByID(_user.Id, _user);

            GroupAddDto added_group = new GroupAddDto()
            {
                Name = "name"
            };

            _mockUOW.GroupRepo.SetGetByID(0, new Group() { Id = 0, Name = "name", User = _user });

            _mockUOW.GroupRepo.SetRemove(It.IsAny<Group>(), true);

            //ACT
            var result = await _groupController.Remove (0);

            //ASSERT
            Assert.That(result.StatusCode, Is.EqualTo(Status204NoContent));
        }

        #endregion
    }
}
