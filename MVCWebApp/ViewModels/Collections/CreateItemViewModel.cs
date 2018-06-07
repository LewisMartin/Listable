using Listable.MVCWebApp.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CreateItemViewModel
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        public bool ImageEnabled { get; set; }

        [Required, MaxLength(30), Display(Name = "Name *")]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [FileSize(4000000)]
        public IFormFile ImageFile { get; set; }
    }
}
