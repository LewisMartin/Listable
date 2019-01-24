using GatewayAPI.Controllers;
using GatewayAPI.Models.Collection;
using GatewayAPI.Models.Collection.Forms;
using GatewayAPI.Tests.Controllers;
using GatewayAPI.Tests.Mocks;
using Listable.CollectionMicroservice.DTO;
using Listable.UserMicroservice.Entities;
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
        protected List<User> DummyUsers;

        protected MockBlobService _MockBlobService;
        protected MockCollectionsService _MockCollectionsService;
        protected MockUserService _MockUserService;
        protected MockImageManipulationService _MockImageManipulationService;

        public CollectionControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            SetUpDummyData();

            _MockBlobService = new MockBlobService();
            _MockCollectionsService = new MockCollectionsService(DummyCollections);
            _MockImageManipulationService = new MockImageManipulationService();
            _MockUserService = new MockUserService(DummyUsers);

            _Controller = new CollectionController(_MockImageManipulationService, _MockBlobService, _MockCollectionsService, _MockUserService);
            _Controller.ControllerContext = _MockControllerContext;
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

        [Test]
        public void QueryCollections_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.QueryCollections(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void QueryCollections_POST_ReturnsNotFound_OnNoMatch()
        {
            // Arrange:

            // Act:
            var res = _Controller.QueryCollections(new CollectionQueryFormModel()
            {
                SearchTerm = "#"
            });

            // Assert:
            Assert.IsInstanceOf<NotFoundResult>(res);
        }

        [Test]
        public void QueryCollections_POST_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.QueryCollections(new CollectionQueryFormModel()
            {
                SearchTerm = DummyCollections.FirstOrDefault().Name
            });

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<CollectionQueryResults>(((JsonResult)res).Value);
        }

        protected override void SetUpDummyData()
        {
            SetUpDummyUsers();
            SetUpDummyCollections();
        }

        protected void SetUpDummyUsers()
        {
            string sub = "";
            foreach (var identity in _MockControllerContext.HttpContext.User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            DummyUsers = new List<User>()
            {
                new User()
                {
                    Id = 1,
                    DisplayName = "Test User",
                    SubjectId = sub
                },
                new User()
                {
                    Id = 2,
                    DisplayName = "User2",
                    SubjectId = "SubjId2"
                }
            };
        }

        protected void SetUpDummyCollections()
        {
            DummyCollections = new List<Collection>()
            {
                new Collection()
                {
                    Id = "1",
                    Owner = DummyUsers[0].Id,
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
                    Owner = DummyUsers[DummyUsers.Count-1].Id,
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
