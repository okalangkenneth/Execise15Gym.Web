using AutoMapper;
using Gym.Core.Entities;
using Gym.Core.Repositories;
using Gym.Core.ViewModels;
using Gym.Data.Data;
using Gym.Tests.Externsions;
using Gym.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Tests.Controller
{
    [TestClass]
    public class GymClassesTests
    {
        private GymClassesController controller;
        private Mock<IGymClassRepository> mockGymClassRepository;
        private Mapper mapper;

        [TestInitialize]
        public void SetUp()
        {
            mockGymClassRepository = new Mock<IGymClassRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.GymClassRepository).Returns(mockGymClassRepository.Object);

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                var profile = new MapperProfile();
                cfg.AddProfile(profile);
            }));

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            controller = new GymClassesController(mockUserManager.Object, mapper, mockUoW.Object);
        }

        [TestMethod]
        public void Index_NotAutenthicated_ReturnsExpected()
        {
            var gymClasses = GetGymClassList();
            var expected = mapper.Map<IndexViewModel>(gymClasses);

            controller.SetUserIsAuthenticated(false);

            mockGymClassRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(gymClasses);
            var vm = new IndexViewModel { ShowHistory = false };

            var viewResult = controller.Index(vm).Result as ViewResult;

            var actual = (IndexViewModel)viewResult.Model;

            Assert.AreEqual(expected.GymClasses.Count(), actual.GymClasses.Count());
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_ShouldPass()
        {
            controller.SetUserIsAuthenticated(true);
            var vm = new IndexViewModel { ShowHistory = false };

            var actual = await controller.Index(vm);

            Assert.IsInstanceOfType(actual, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_ReturnsDefaultView_ShouldReturnNull()
        {
            controller.SetAjaxRequest(false);
            var actual = controller.Create() as ViewResult;

            Assert.IsNull(actual.ViewName);

        }

        [TestMethod]
        public void Create_ReturnsPartialViewWhenAjax_ShouldNotBeNull()
        {
            controller.SetAjaxRequest(true);
            var actual = controller.Create() as PartialViewResult;

            Assert.IsNotNull(actual);
        }


        private List<GymClass> GetGymClassList()
        {
            return new List<GymClass>
            {
                new GymClass
                {
                    Id = 1,
                    Name = "Spinning",
                    Description = "Easy",
                    StartDate = DateTime.Now.AddDays(3),
                    Duration = new TimeSpan(0,60,0)
                },
                new GymClass
                {
                    Id = 2,
                    Name = "HyperFys",
                    Description = "Hard",
                    StartDate = DateTime.Now.AddDays(-3),
                    Duration = new TimeSpan(0,60,0)
                }
            };
        }
    }
}
