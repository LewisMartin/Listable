using Microsoft.AspNetCore.Http;

namespace Listable.MVCWebApp.Services
{
    public interface IImageManipulation
    {
        ImageManipulation LoadFile(IFormFile image);

        ImageManipulation Resize(int targetWidth);

        IFormFile Retrieve();
    }
}