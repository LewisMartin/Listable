using Listable.BlobMicroservice.Controllers;
using Listable.BlobMicroservice.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Listable.BlobMicroservice.Tests.Controllers
{
    [TestFixture]
    class ImageServiceControllerTests
    {
        private ImageServiceController _Controller;

        public ImageServiceControllerTests() { }

        [SetUp]
        public void SetUpAttribute()
        {
            _Controller = new ImageServiceController(new MockImageStore());
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
