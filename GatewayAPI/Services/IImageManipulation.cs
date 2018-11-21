using Microsoft.AspNetCore.Http;

namespace GatewayAPI.Services
{
    public interface IImageManipulation
    {
        IImageManipulation LoadFile(IFormFile image);

        IImageManipulation Resize(int targetWidth);

        IFormFile Retrieve();
    }
}