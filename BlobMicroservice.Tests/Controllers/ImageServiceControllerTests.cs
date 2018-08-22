using Listable.BlobMicroservice.Controllers;
using Listable.BlobMicroservice.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        [Test]
        public void Upload_POST_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Upload(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res.Result);
        }

        [Test]
        public void Upload_POST_ReturnsJsonResult_WithImageIdString()
        {
            // Arrange:
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            fileMock.Setup(_ => _.OpenReadStream()).Returns(new MemoryStream());
            fileMock.Setup(_ => _.FileName).Returns("dummy.jpg");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            // Act:
            var res = _Controller.Upload(fileMock.Object);

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res.Result);
            Assert.IsInstanceOf<string>(((JsonResult)res.Result).Value);
        }

        [Test]
        public void RetrieveUrl_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange: 

            // Act:
            var res = _Controller.RetrieveUrl(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void RetrieveUrl_GET_ReturnsJsonResult_WithImageUrlString()
        {
            // Arrange: 

            // Act:
            var res = _Controller.RetrieveUrl(Guid.NewGuid().ToString());

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<string>(((JsonResult)res).Value);
        }

        [Test]
        public void RetrieveThumbnailUrls_GET_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange: 

            // Act:
            var res = _Controller.RetrieveThumbnailUrls(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res);
        }

        [Test]
        public void RetrieveThumbnailUrls_GET_ReturnsJsonResult_WithThumbnailDictionary()
        {
            // Arrange: 

            // Act:
            var res = _Controller.RetrieveThumbnailUrls(new string[] { "1" });

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res);
            Assert.IsInstanceOf<Dictionary<string, string>>(((JsonResult)res).Value);
        }

        [Test]
        public void Delete_DELETE_ReturnsBadRequest_OnNullParameter()
        {
            // Arrange:

            // Act:
            var res = _Controller.Delete(null);

            // Assert:
            Assert.IsInstanceOf<BadRequestResult>(res.Result);
        }

        [Test]
        public void Delete_DELETE_ReturnsJsonResult_WithBool()
        {
            // Arrange:

            // Act:
            var res = _Controller.Delete("1");

            // Assert:
            Assert.IsInstanceOf<JsonResult>(res.Result);
            Assert.IsInstanceOf<bool>(((JsonResult)res.Result).Value);
        }
    }
}
