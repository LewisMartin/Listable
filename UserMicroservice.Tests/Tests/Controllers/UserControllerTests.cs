using System;
using System.Linq;
using Listable.UserMicroservice.Controllers;
using Listable.UserMicroservice.DTO;
using Listable.UserMicroservice.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Listable.UserMicroservice.Tests.Controllers
{
    [TestFixture]
    class UserControllerTests
    {
        private UserController _Controller;
        private UserDbContext _context;

        private User _dummyUser;

        public UserControllerTests() { }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "Users")
                .Options;

            _dummyUser = new User() { SubjectId = "subjid1", DisplayName = "Test User", FirstName = "Test", LastName = "User" };

            _context = new UserDbContext(options);

            _context.Users.Add(_dummyUser);
            _context.SaveChanges();

            _Controller = new UserController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            if (_context.Database.IsInMemory())
            {
                _context.Database.EnsureDeleted();
            }
        }

        [Test]
        public void Authorize_Attribute_Exists()
        {
            // Arrange:
            Type type = _Controller.GetType();

            // Act:
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [Test]
        public void GetUser_GET_ReturnsNotFound_WhenUserDoesntExist()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetUser(0);

            // Asset:
            Assert.IsInstanceOf<NotFoundResult>(res);
        }

        [Test]
        public void GetUser_GET_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetUser(_dummyUser.Id);

            // Asset:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<User>(((JsonResult)res).Value);
        }

        [Test]
        public void GetUserBySub_GET_ReturnsBadRequest_WhenSubjectIdNull()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetUserBySub(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void GetUserBySub_GET_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetUserBySub(_dummyUser.SubjectId);

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<User>(((JsonResult)res).Value);
        }

        [Test]
        public void CheckForUserEntry_GET_ReturnsBadRequest_OnSubjectIdNull()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckForUserEntry(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void ChecKForUserEntry_GET_ReturnsNotFound_WhenEntryDoesntExists()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckForUserEntry(_dummyUser.SubjectId);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void ChecKForUserEntry_GET_ReturnsOK_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckForUserEntry(_dummyUser.SubjectId);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void CreateUser_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.CreateUser(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CreateUser_POST_ReturnsOk_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.CreateUser(new UserDetails()
            {
                SubjectId = "newSubjectId",
                DisplayName = "NewUser"
            });

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void UpdateUser_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.UpdateUser(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void UpdateUser_POST_ReturnsOK_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.UpdateUser(new UserDetails()
            {
                Id = _dummyUser.Id,
                DisplayName = "Updated Name",
                FirstName = "New",
                LastName = "Name"
            });

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void DeleteUser_DELETE_ReturnsNotFound_WhenUserDoesntExist()
        {
            // Arrange:

            // Act:
            var res = _Controller.DeleteUser(0);

            // Assert:
            Assert.IsInstanceOf<NotFoundResult>(res);
        }

        [Test]
        public void DeleteUser_DELETE_ReturnsOk_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.DeleteUser(_dummyUser.Id);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void CheckDisplayName_GET_ReturnsBadRequest_OnDuplicateDisplayName()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckDisplayName(_dummyUser.DisplayName);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CheckDisplayName_GET_ReturnsOK_OnUniqueDisplayName()
        {
            // Arrange:
            var chars = _dummyUser.DisplayName.ToCharArray();
            Array.Reverse(chars);

            var uniqueName = new string(chars);

            // Act:
            var res = _Controller.CheckDisplayName(uniqueName);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }
    }
}
