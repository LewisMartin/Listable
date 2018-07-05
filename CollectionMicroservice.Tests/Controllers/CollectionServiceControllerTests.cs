using Listable.CollectionMicroservice.Controllers;
using Listable.CollectionMicroservice.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Listable.CollectionMicroservice.Tests.Controllers
{
    [TestFixture]
    class CollectionServiceControllerTests
    {
        private CollectionServiceController _Controller;

        public CollectionServiceControllerTests() { }

        [SetUp]
        public void SetUp()
        {
            _Controller = new CollectionServiceController(new MockCollectionStore());
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
    }
}
