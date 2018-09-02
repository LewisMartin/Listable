using Listable.MVCWebApp.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class ItemEditor
    {
        public string CollectionId { get; set; }

        public bool ImageEnabled { get; set; }

        [Required, MaxLength(30), Display(Name = "Name *")]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [FileSize(8000000)]
        [FileType("jpg,jpeg,png,gif,bmp")]
        public IFormFile ImageFile { get; set; }
    }
}
