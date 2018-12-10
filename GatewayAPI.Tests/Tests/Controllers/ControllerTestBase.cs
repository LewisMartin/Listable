using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace GatewayAPI.Tests.Controllers
{
    [TestFixture]
    public abstract class ControllerTestBase<T> where T : Controller
    {
        protected ControllerContext _MockControllerContext;
        protected T _Controller;

        public ControllerTestBase()
        {
            CreateMockHttpContext();
        }

        [SetUp]
        public abstract void SetUp();

        [TearDown]
        public void TearDown()
        {
            _Controller.ModelState.Clear();
            TearDownSpecific();
        }

        public virtual void TearDownSpecific() { }

        protected void CreateMockHttpContext()
        {
            var MockHttpContext = new Mock<HttpContext>();
            MockHttpContext.Setup(t => t.User).Returns(CreateMockUser());

            _MockControllerContext = new ControllerContext();
            _MockControllerContext.HttpContext = MockHttpContext.Object;
        }

        protected ClaimsPrincipal CreateMockUser()
        {
            IList<Claim> MockClaims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "TestUser")
            };

            var Identity = new ClaimsIdentity(MockClaims, "TestAuthType");

            return new ClaimsPrincipal(Identity);
        }

        protected void SimulateValidation(object model)
        {
            // mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                _Controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }
}
