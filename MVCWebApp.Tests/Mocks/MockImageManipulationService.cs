using Listable.MVCWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Listable.MVCWebApp.Tests.Mocks
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
