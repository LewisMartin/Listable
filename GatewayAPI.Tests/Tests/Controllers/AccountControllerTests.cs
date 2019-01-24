using GatewayAPI.Controllers;
using GatewayAPI.Models.Account;
using GatewayAPI.Tests.Mocks;
using Listable.UserMicroservice.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GatewayAPI.Tests.Controllers
{
    [TestFixture]
    class AccountControllerTests : ControllerTestBase<AccountController>
    {
        protected MockUserService _MockUserService;
        protected List<User> DummyUsers;

        public AccountControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            SetUpDummyData();

            _MockUserService = new MockUserService(DummyUsers);
            _Controller = new AccountController(_MockUserService);
            _Controller.ControllerContext = _MockControllerContext;
        }

        [Test]
        public void GetProfile_GET_ReturnsServerError_OnFailure()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetProfile((DummyUsers.Max(u => u.Id)) + 1);

            // Assert:
            Assert.IsInstanceOf<StatusCodeResult>(res);

            var statusCodeResult = res as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Test]
        public void GetProfile_GET_ReturnsOk_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetProfile((DummyUsers.FirstOrDefault().Id));

            // Assert:
            Assert.IsInstanceOf<OkObjectResult>(res);
            Assert.IsInstanceOf<ProfileView>(((OkObjectResult)res).Value);
        }

        [Test]
        public void GetProfileForAuthenticatedUser_GET_ReturnsServerError_OnFailure()
        {
            // Arrange:
            _MockUserService.DummyUsers.RemoveAll(u => u.SubjectId == GetUserSub());

            // Act:
            var res = _Controller.GetProfileForAuthenticatedUser();

            // Assert:
            Assert.IsInstanceOf<StatusCodeResult>(res);

            var statusCodeResult = res as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Test]
        public void GetProfileForAuthenticatedUser_GET_ReturnsOk_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetProfileForAuthenticatedUser();

            // Assert:
            Assert.IsInstanceOf<OkObjectResult>(res);
            Assert.IsInstanceOf<ProfileView>(((OkObjectResult)res).Value);
        }

        [Test]
        public void EditProfile_POST_ReturnsForbidResult_OnAuthorizationIssue()
        {
            // Arrange:

            // Act:
            var res = _Controller.EditProfile(new EditProfileFormModel()
            {
                Id = _MockUserService.DummyUsers.FirstOrDefault(u => u.SubjectId != GetUserSub()).Id,
                DisplayName = "Username"
            });

            // Assert:
            Assert.IsInstanceOf<ForbidResult>(res);
        }

        [Test]
        public void EditProfile_POST_ReturnsServerError_OnFailure()
        {
            // Arrange:
            int deletedUserId = _MockUserService.DummyUsers.FirstOrDefault(u => u.SubjectId == GetUserSub()).Id;
            _MockUserService.DummyUsers.RemoveAll(u => u.Id == deletedUserId);

            // Act:
            var res = _Controller.EditProfile(new EditProfileFormModel()
            {
                Id = deletedUserId,
                DisplayName = "Username"
            });

            // Assert:
            Assert.IsInstanceOf<StatusCodeResult>(res);

            var statusCodeResult = res as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Test]
        public void EditProfile_POST_ReturnsOk_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.EditProfile(new EditProfileFormModel()
            {
                Id = _MockUserService.DummyUsers.FirstOrDefault(u => u.SubjectId == GetUserSub()).Id,
                DisplayName = "Username"
            });

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        // [HttpGet] public IActionResult CheckDisplayName(string displayName)
        [Test]
        public void CheckDisplayName_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckDisplayName(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CheckDisplayName_GET_ReturnsBadRequest_WhenDisplayNameTaken()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckDisplayName(DummyUsers.FirstOrDefault().DisplayName);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CheckDisplayName_GET_ReturnsOk_WhenDisplayNameFree()
        {
            // Arrange:

            // Act:
            var res = _Controller.CheckDisplayName("untakendisplayname12");

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        protected override void SetUpDummyData()
        {
            DummyUsers = new List<User>()
            {
                new User()
                {
                    Id = 1,
                    DisplayName = "Test User",
                    SubjectId = GetUserSub()
                },
                new User()
                {
                    Id = 2,
                    DisplayName = "User2",
                    SubjectId = "SubjId2"
                }
            };
        }

        protected string GetUserSub()
        {
            string sub = "";

            foreach (var identity in _MockControllerContext.HttpContext.User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            return sub;
        }
    }
}
