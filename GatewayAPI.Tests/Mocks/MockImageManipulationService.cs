using GatewayAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;

namespace GatewayAPI.Tests.Mocks
{
    public class MockImageManipulationService : IImageManipulation
    {
        public IImageManipulation LoadFile(IFormFile image)
        {
            return this;
        }

        public IImageManipulation Resize(int targetWidth)
        {
            return this;
        }

        public IFormFile Retrieve()
        {
            return new FormFile(new MemoryStream(), 0, 1, "Dummy", "FormFile");
        }
    }
}
