using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Listable.MVCWebApp.CustomAttributes
{
    public class FileTypeAttribute : CustomAttribute
    {
        private IEnumerable<string> _ValidTypes { get; set; }

        public string ValidTypes { get; set; }

        public FileTypeAttribute(string validTypes)
        {
            _ValidTypes = validTypes.Split(",").ToList();
            ErrorMessage = "Only the following file types are allowed: " + validTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                if (!_ValidTypes.Any(e => file.FileName.EndsWith(e)))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
