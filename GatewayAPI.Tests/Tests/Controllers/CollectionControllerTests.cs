using GatewayAPI.Controllers;
using GatewayAPI.Models.Collection.Forms;
using GatewayAPI.Tests.Controllers;
using GatewayAPI.Tests.Mocks;
using Listable.CollectionMicroservice.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GatewayAPI.Tests.Tests.Controllers
{
    [TestFixture]
    class CollectionControllerTests : ControllerTestBase<CollectionController>
    {
        protected List<Collection> DummyCollections;

        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;
        protected MockImageManipulationService _MockImageManipulationService;

        public CollectionControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            SetUpDummyData();

            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService(DummyCollections);
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
            var result = _Controller.GetCollections(null);

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void GetCollection_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollection(null).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void GetCollectionSettings_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollectionSettings(null);

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void GetCollectionItem_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollectionItem(null, null).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
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

        [Test]
        public void GetCollection_ReturnsOkResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollection(DummyCollections.FirstOrDefault().Id).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetCollections_ReturnsOkResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollections(DummyCollections.FirstOrDefault().Owner);

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetCollectionSettings_ReturnsOkResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollectionSettings(DummyCollections.FirstOrDefault().Id);

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void CreateCollection_ReturnsOkResult()
        {
            // Arrange:
            var model = new CreateCollectionFormModel()
            {
                Name = "New Collection",
                ImageEnabled = false,
                GridDisplay = false
            };

            // Act:
            var result = _Controller.CreateCollection(model);

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void EditCollection_ReturnsOkResult()
        {
            // Arrange:
            var model = new EditCollectionFormModel()
            {
                Id = DummyCollections.FirstOrDefault().Id,
                Name = "Edited Collection",
                GridDisplay = false
            };

            // Act:
            var result = _Controller.EditCollection(model).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void DeleteCollection_ReturnsOkResult()
        {
            // Arrange:
            var model = new DeleteCollectionFormModel()
            {
                SelectedCollectionId = DummyCollections.FirstOrDefault().Id
            };

            // Act:
            var result = _Controller.DeleteCollection(model).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void GetCollectionItem_ReturnsOkResult()
        {
            // Arrange:

            // Act:
            var result = _Controller.GetCollectionItem(DummyCollections.FirstOrDefault().Id, 
                DummyCollections.FirstOrDefault().CollectionItems.FirstOrDefault().Id.ToString()).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void CreateCollectionItem_ReturnsOkResult()
        {
            // Arrange:
            var model = new CreateCollectionItemFormModel()
            {
                CollectionId = DummyCollections.FirstOrDefault().Id,
                Name = "New Item",
                Description = "Desc."
            };

            // Act:
            var result = _Controller.CreateCollectionItem(model).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void EditCollectionItem_ReturnsOkResult()
        {
            // Arrange:
            var model = new EditCollectionItemFormModel()
            {
                CollectionId = DummyCollections.FirstOrDefault().Id,
                Id = DummyCollections.FirstOrDefault().CollectionItems.FirstOrDefault().Id.ToString(),
                Name = "Edited Item",
                Description = "Updated Desc."
            };

            // Act:
            var result = _Controller.EditCollectionItem(model).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void DeleteCollectionItem_ReturnsOkResult()
        {
            // Arrange:
            var model = new DeleteCollectionItemFormModel()
            {
                CollectionId = DummyCollections.FirstOrDefault().Id,
                CollectionItemId = DummyCollections.FirstOrDefault().CollectionItems.FirstOrDefault().Id.ToString()
            };

            // Act:
            var result = _Controller.DeleteCollectionItem(model).Result;

            // Assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkResult>(result);
        }

        protected override void SetUpDummyData()
        {
            DummyCollections = new List<Collection>()
            {
                new Collection()
                {
                    Id = "1",
                    Owner = "TestUser",
                    Name = "Collection 1",
                    ImageEnabled = false,
                    DisplayFormat = CollectionDisplayFormat.List,
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 1",
                            Description = "Item 1 description",
                            ImageId = null
                        }
                    }
                },
                new Collection()
                {
                    Id = "2",
                    Owner = "TestUser",
                    Name = "Collection 2",
                    ImageEnabled = true,
                    DisplayFormat = CollectionDisplayFormat.Grid,
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 1",
                            Description = "Item 1 description",
                            ImageId = "1"
                        }
                    }
                }
            };
        }
    }
}
