using Moq;
using System;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Listable.MVCWebApp.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Listable.MVCWebApp.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using Listable.MVCWebApp.ViewModels.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Listable.MVCWebApp.Tests
{
    [TestFixture]
    public class CollectionsControllerTests
    {
        protected ControllerContext _MockControllerContext;
        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;

        protected CollectionsController _Controller;

        public CollectionsControllerTests() { }

        [SetUp]
        public void SetUp()
        {
            CreateMockHttpContext();
            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService();

            _Controller = new CollectionsController(_MockBlobService, _MockCollectionsService);
            _Controller.ControllerContext = _MockControllerContext;
        }

        [Test]
        public void Authorize_Attribute_Exists()
        {
            // Arrange:

            // Act:
            Type type = _Controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }

        [Test]
        public void Index_GET_ReturnsARedirectToOverviewAction()
        {
            // Arrange:

            // Act:
            var result = _Controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
        }

        [Test]
        public void Overview_GET_ReturnsAViewResult_WithAListOfCollections()
        {
            // Arrange:

            // Act:
            var result = _Controller.Overview().Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OverviewViewModel>(result.Model);
        }

        [Test]
        public void Collection_GET_ReturnsAViewResult_WithCollectionViewModel()
        {
            // Arrange:

            // Act:
            var result = _Controller.Collection("1").Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CollectionViewModel>(result.Model);
        }

        [Test]
        public void Collection_GET_ReturnsARedirectToCollectionGridAction_WhenCollectionIsGridType()
        {
            // Arrange:

            // Act:
            var result = _Controller.Collection("2").Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CollectionGrid", result.ActionName);
        }

        [Test]
        public void CollectionGrid_GET_ReturnsAViewResult_WithCollectionGridViewModel()
        {
            // Arrange:

            // Act:
            var result = _Controller.CollectionGrid("2").Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CollectionGridViewModel>(result.Model);
        }

        [Test]
        public void CreateCollection_GET_ReturnsAViewResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.CreateCollection() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateCollection_POST_ReturnsARedirectToOverviewAction_WhenModelStateIsValid()
        {
            // Arrange:

            // Act:
            var result = _Controller.CreateCollection(new CreateCollectionViewModel()
            {
                Name = "New collection",
                IsImageEnabled = false,
                GridDisplay = false
            }).Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
        }

        [Test]
        public void CreateCollection_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new CreateCollectionViewModel()
            {
                Name = null,
                IsImageEnabled = false,
                GridDisplay = false
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.CreateCollection(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CreateCollectionViewModel>(result.Model);
        }

        [Test]
        public void DeleteCollection_GET_ReturnsAViewResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.DeleteCollection().Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteCollection_POST_ReturnsARedirectToOverviewAcrion_WhenModelStateIsValid()
        {
            // Arrange:

            // Act:
            var result = _Controller.DeleteCollection(new DeleteCollectionViewModel()
            {
                SelectedCollection = null,
                Collections = new List<SelectListItem>()
            }).Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
        }

        [Test]
        public void DeleteCollection_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new DeleteCollectionViewModel()
            {
                SelectedCollection = null,
                Collections = new List<SelectListItem>()
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.DeleteCollection(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DeleteCollectionViewModel>(result.Model);
        }

        private void CreateMockHttpContext()
        {
            var MockHttpContext = new Mock<HttpContext>();
            MockHttpContext.Setup(t => t.User).Returns(CreateMockUser());

            _MockControllerContext = new ControllerContext();
            _MockControllerContext.HttpContext = MockHttpContext.Object;
        }

        private ClaimsPrincipal CreateMockUser()
        {
            IList<Claim> MockClaims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "TestUser")
            };

            var Identity = new ClaimsIdentity(MockClaims, "TestAuthType");

            return new ClaimsPrincipal(Identity);
        }

        private void SimulateValidation(object model)
        {
            // mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                _Controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }
}
