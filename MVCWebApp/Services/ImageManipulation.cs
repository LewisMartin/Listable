using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http.Headers;

namespace Listable.MVCWebApp.Services
{
    public class ImageManipulation : IImageManipulation
    {
        private Image _sourceImage;
        private string _fileName;

        public ImageManipulation() { }

        public IImageManipulation LoadFile(IFormFile image)
        {
            _sourceImage = Image.FromStream(image.OpenReadStream());
            _fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName;

            return this;
        }

        public IImageManipulation Resize(int targetWidth)
        {
            if (_sourceImage != null)
            {
                int targetHeight = (_sourceImage.Height * targetWidth / _sourceImage.Width);

                Image destImage = new Bitmap(targetWidth, targetHeight);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(
                        _sourceImage,
                        0,
                        0,
                        targetWidth,
                        targetHeight);
                }

                _sourceImage = destImage;

                return this;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public IFormFile Retrieve()
        {
            if (_sourceImage != null)
                return ImageToFormFile(_sourceImage);
            else
                throw new NullReferenceException();
        }

        private IFormFile ImageToFormFile(Image image)
        {
            Stream resizedImgStream = new MemoryStream();
            image.Save(resizedImgStream, ImageFormat.Jpeg);

            return new FormFile(resizedImgStream, 0, resizedImgStream.Length, "image", _fileName);
        }
    }
}
