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

        [Required, MaxLength(30), Display(Name = "Name *")]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }
    }
}
