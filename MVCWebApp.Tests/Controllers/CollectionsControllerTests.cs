using Moq;
using System;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Listable.MVCWebApp.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Listable.MVCWebApp.Tests.Mocks;

namespace Listable.MVCWebApp.Tests
{
    [TestFixture]
    public class CollectionsControllerTests
    {
        public CollectionsControllerTests() { }

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void AuthorizeAttributeExists()
        {
            // Arrange:
            CollectionsController controller = new CollectionsController(new MockBlobService(), new MockCollectionsService());

            // Act:
            Type type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert:
            Assert.IsTrue(attributes.Any());
        }
    }
}
