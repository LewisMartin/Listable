using Moq;
using System;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Listable.MVCWebApp.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Listable.MVCWebApp.Tests.Mocks;
using System.Collections.Generic;
using Listable.MVCWebApp.ViewModels.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Listable.MVCWebApp.Tests.Controllers
{
    [TestFixture]
    public class CollectionsControllerTests : ControllerTestBase<CollectionsController>
    {
        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;

        public CollectionsControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService();

            _Controller = new CollectionsController(_MockBlobService, _MockCollectionsService);
            _Controller.ControllerContext = _MockControllerContext;
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
        public void Overview_GET_ReturnsAViewResult_WithOverviewViewModel()
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
            Assert.IsInstanceOf<CreateCollectionViewModel>(result.Model);
        }

        [Test]
        public void CreateCollection_POST_ReturnsARedirectToOverviewAction_WhenModelStateIsValid()
        {
            // Arrange:

            // Act:
            var result = _Controller.CreateCollection(new CreateCollectionViewModel()
            {
                CollectionDetails = new CollectionEditor()
                {
                    Name = "New collection",
                    IsImageEnabled = false,
                    GridDisplay = false
                }
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
                CollectionDetails = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.CreateCollection(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CreateCollectionViewModel>(result.Model);
        }

        [Test]
        public void EditCollection_GET_ReturnsAViewResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.EditCollection("1").Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EditCollectionViewModel>(result.Model);
        }

        [Test]
        public void EditCollection_POST_ReturnsARedirectToOverviewAction_WhenModelStateIsValid()
        {
            // Arrange:

            // Act:
            var result = _Controller.EditCollection(new EditCollectionViewModel()
            {
                CollectionId = "1",
                CollectionDetails = new CollectionEditor()
                {
                    Name = "Updated Collection",
                    IsImageEnabled = true,
                    GridDisplay = false
                }
            }).Result as RedirectToActionResult;

            // Assert:
            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
        }

        [Test]
        public void EditCollection_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new EditCollectionViewModel()
            {
                CollectionId = null,
                CollectionDetails = new CollectionEditor()
                {
                    Name = null,
                    IsImageEnabled = false,
                    GridDisplay = false
                }
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.EditCollection(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EditCollectionViewModel>(result.Model);
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
            var model = new DeleteCollectionViewModel()
            {
                SelectedCollection = "1",
                Collections = new List<SelectListItem>()
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.DeleteCollection(model).Result as RedirectToActionResult;

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
    }
}
