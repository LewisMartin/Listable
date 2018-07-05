using Microsoft.AspNetCore.Http;

namespace Listable.MVCWebApp.Services
{
    public interface IImageManipulation
    {
        IImageManipulation LoadFile(IFormFile image);

        IImageManipulation Resize(int targetWidth);

        IFormFile Retrieve();
    }
}