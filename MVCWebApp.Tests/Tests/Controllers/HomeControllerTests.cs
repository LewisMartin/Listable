using Listable.MVCWebApp.Controllers;
using Listable.MVCWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Listable.MVCWebApp.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests : ControllerTestBase<HomeController>
    {
        public HomeControllerTests() : base() { }

        [SetUp]
        public override void SetUp()
        {
            _Controller = new HomeController();
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
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Collections", result.ControllerName);
        }

        [Test]
        public void Error_GET_ReturnsAViewResult_WithErrorViewModel()
        {
            // Arrange:

            // Act:
            var result = _Controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErrorViewModel>(result.Model);
        }
    }
}
