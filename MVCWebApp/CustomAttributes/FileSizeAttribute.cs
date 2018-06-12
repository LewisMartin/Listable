using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.CustomAttributes
{
    public class FileSizeAttribute : CustomAttribute
    {
        public int? _maxBytes { get; set; } 

        public FileSizeAttribute(int maxBytes)
        {
            _maxBytes = maxBytes;
            if (_maxBytes.HasValue)
            {
                ErrorMessage = "Please upload a file of less than " + (_maxBytes.Value/1000000) + " MB.";
            }
        }

        public override bool IsValid(object value)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                bool result = true;

                if (_maxBytes.HasValue)
                {
                    result &= (file.Length < _maxBytes.Value);
                }

                return result;
            }

            return true;
        }
    }
}
