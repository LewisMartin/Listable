using Listable.CollectionMicroservice.Controllers;
using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Listable.CollectionMicroservice.Tests.Controllers
{
    [TestFixture]
    class CollectionServiceControllerTests
    {
        private CollectionServiceController _Controller;

        public CollectionServiceControllerTests() { }

        private Collection _dummyCollection;
        private CollectionItem _dummyCollectionItem;

        private MockCollectionStore _mockCollectionStore;

        [SetUp]
        public void SetUp()
        {
            _dummyCollectionItem = new CollectionItem()
            {
                Id = Guid.NewGuid(),
                Name = "Mock Item",
                Description = "A mock collection item",
                ImageId = "1"
            };

            _dummyCollection = new Collection()
            {
                Id = "1",
                Owner = 1,
                Name = "Collection1",
                ImageEnabled = false,
                DisplayFormat = CollectionDisplayFormat.List,
                CollectionItems = new List<CollectionItem>()
                    {
                        _dummyCollectionItem
                    }
            };

            _mockCollectionStore = new MockCollectionStore();
            _Controller = new CollectionServiceController(_mockCollectionStore);
        }

        [TearDown]
        public void TearDown()
        {
            _mockCollectionStore.ClearCollections();
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
        public void Retrieve_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Retrieve(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void Retrieve_GET_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.Retrieve(_dummyCollection.Id);

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<Collection>(((JsonResult)res).Value);
        }

        [Test]
        public void RetrieveAll_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.RetrieveAll(0);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void RetrieveAll_GET_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.RetrieveAll(1);

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<IEnumerable<Collection>>(((JsonResult)res).Value);
        }

        [Test]
        public void RetrieveItem_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.RetrieveItem(null, null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void RetrieveItem_GET_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.RetrieveItem(_dummyCollection.Id, _dummyCollectionItem.Id.ToString());

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<CollectionItem>(((JsonResult)res).Value);
        }

        [Test]
        public void Create_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Create(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void Create_POST_ReturnsJsonResult_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.Create(_dummyCollection);

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
        }

        [Test]
        public void Update_PUT_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Update(null, null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void Update_PUT_ReturnsOkResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.Update(_dummyCollection.Id, _dummyCollection);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void Delete_DELETE_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Delete(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void Delete_DELETE_ReturnsOkResult_OnSuccess()
        {
            // Arrange:

            // Act:
            var res = _Controller.Delete(_dummyCollection.Id);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void CreateItem_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.CreateItem(null, null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void CreateItem_POST_ReturnsOkResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.CreateItem(_dummyCollection.Id, _dummyCollectionItem);

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }

        [Test]
        public void DeleteItem_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.DeleteItem(null, null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void DeleteItem_POST_ReturnsOkResult_OnSuccess()
        {
            // Arrange:
            _Controller.Create(_dummyCollection);

            // Act:
            var res = _Controller.DeleteItem(_dummyCollection.Id, new List<string>() { _dummyCollectionItem.Id.ToString() });

            // Assert:
            Assert.IsInstanceOf<OkResult>(res);
        }
    }
}
