using GatewayAPI.Controllers;
using GatewayAPI.Tests.Controllers;
using GatewayAPI.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
