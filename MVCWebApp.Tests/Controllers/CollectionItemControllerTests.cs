using Listable.MVCWebApp.Controllers;
using Listable.MVCWebApp.Tests.Mocks;
using Listable.MVCWebApp.ViewModels.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Listable.MVCWebApp.Tests.Controllers
{
    [TestFixture]
    public class CollectionItemControllerTests : ControllerTestBase<CollectionItemController>
    {
        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;
        protected MockImageManipulationService _MockImageManipulationService;

        public CollectionItemControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService();
            _MockImageManipulationService = new MockImageManipulationService();

            _Controller = new CollectionItemController(_MockImageManipulationService, _MockBlobService, _MockCollectionsService);
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
            Assert.AreEqual("Collections", result.ControllerName);
        }

        [Test]
        public void ViewItem_GET_ReturnsAViewResult_WithViewItemViewModel()
        {
            // Arrange:
            var DummyCollectionId = _MockCollectionsService.DummyCollections[0].Id;
            var DummyItemId = _MockCollectionsService.DummyCollections[0].CollectionItems[0].Id.ToString();

            // Act:
            var result = _Controller.ViewItem(DummyCollectionId, DummyItemId).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewItemViewModel>(result.Model);
        }

        [Test]
        public void CreateItem_GET_ReturnsAViewResult_WithCreateItemViewModel()
        {
            // Arrange:

            // Act:
            var result = _Controller.CreateItem("1").Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CreateItemViewModel>(result.Model);
        }

        [Test]
        public void CreateItem_POST_ReturnsARedirectToCollectionAction_WhenModelStateIsValid()
        {
            // Arrange:
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            fileMock.Setup(_ => _.OpenReadStream()).Returns(new MemoryStream());
            fileMock.Setup(_ => _.FileName).Returns("dummy.jpg");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var model = new CreateItemViewModel()
            {
                CollectionName = "Collection Name",
                ItemDetails = new ItemEditor()
                {
                    CollectionId = "1",
                    ImageEnabled = false,
                    Name = "Item 1",
                    Description = "Item 1 Description",
                    ImageFile = fileMock.Object
                }
            };

            // Act:
            var result = _Controller.CreateItem(model).Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Collection", result.ActionName);
            Assert.AreEqual("Collections", result.ControllerName);
        }

        [Test]
        public void CreateItem_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new CreateItemViewModel()
            {
                ItemDetails = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.CreateItem(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CreateItemViewModel>(result.Model);
        }

        [Test]
        public void EditItem_GET_ReturnsAViewResult_WithEditItemViewModel()
        {
            // Arrange:
            var collectionId = _MockCollectionsService.DummyCollections.First().Id;
            var itemId = _MockCollectionsService.DummyCollections.First().CollectionItems.First().Id;

            // Act:
            var result = _Controller.EditItem(collectionId, itemId.ToString()).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EditItemViewModel>(result.Model);
        }

        [Test]
        public void EditItem_POST_ReturnsARedirectToCollectionAction_WhenModelStateIsValid()
        {
            // Arrange:
            var model = new EditItemViewModel()
            {
                CollectionName = "Collection Name",
                ItemId = _MockCollectionsService.DummyCollections.First().CollectionItems.First().Id.ToString(),
                ItemDetails = new ItemEditor()
                {
                    CollectionId = _MockCollectionsService.DummyCollections.First().Id,
                    ImageEnabled = false,
                    Name = "Item 1",
                    Description = "Item 1 Description",
                    ImageFile = null
                }
            };

            // Act:
            var result = _Controller.EditItem(model).Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Collection", result.ActionName);
            Assert.AreEqual("Collections", result.ControllerName);
        }

        [Test]
        public void EditItem_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new EditItemViewModel()
            {
                ItemDetails = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.EditItem(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EditItemViewModel>(result.Model);
        }

        [Test]
        public void DeleteItem_GET_ReturnsAViewResult_WithDeleteItemViewModel()
        {
            // Arrange:

            // Act:
            var result = _Controller.DeleteItem("1").Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DeleteItemViewModel>(result.Model);
        }

        [Test]
        public void DeleteItem_POST_ReturnsARedirectToCollectionAction_WhenModelStateIsValid()
        {
            // Arrange:
            var model = new DeleteItemViewModel()
            {
                CollectionId = "1",
                CollectionName = "Name",
                DeleteItemOptions = new List<DeleteItemOption>()
            };

            // Act:
            var result = _Controller.DeleteItem(model).Result as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Collection", result.ActionName);
            Assert.AreEqual("Collections", result.ControllerName);
        }

        [Test]
        public void DeleteItem_POST_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            // Arrange:
            var model = new DeleteItemViewModel()
            {
                CollectionId = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.DeleteItem(model).Result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DeleteItemViewModel>(result.Model);
        }
    }
}
