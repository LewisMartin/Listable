using GatewayAPI.Controllers;
using GatewayAPI.Models.Collection.Forms;
using GatewayAPI.Tests.Controllers;
using GatewayAPI.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Linq;

namespace GatewayAPI.Tests.Tests.Controllers
{
    [TestFixture]
    class CollectionControllerTests : ControllerTestBase<CollectionController>
    {
        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;
        protected MockImageManipulationService _MockImageManipulationService;

        public CollectionControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService();
            _MockImageManipulationService = new MockImageManipulationService();

            _Controller = new CollectionController(_MockImageManipulationService, _MockBlobService, _MockCollectionsService);
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
        public void GetCollections_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetCollections(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void GetCollection_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetCollection(null).Result;

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void GetCollectionSettings_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetCollectionSettings(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void GetCollectionItem_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.GetCollectionItem(null, null).Result;

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CreateCollection_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new CreateCollectionFormModel()
            {
                Name = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.CreateCollection(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void EditCollection_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new EditCollectionFormModel()
            {
                Id = null,
                Name = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.EditCollection(model).Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void DeleteCollection_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new DeleteCollectionFormModel()
            {
                SelectedCollectionId = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.DeleteCollection(model).Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void CreateCollectionItem_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new CreateCollectionItemFormModel()
            {
                CollectionId = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.CreateCollectionItem(model).Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void EditCollectionItem_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new EditCollectionItemFormModel()
            {
                CollectionId = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.EditCollectionItem(model).Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void DeleteCollectionItem_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange:
            var model = new DeleteCollectionItemFormModel()
            {
                CollectionId = null
            };

            // Act:
            SimulateValidation(model);
            var result = _Controller.DeleteCollectionItem(model).Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
