using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CreateCollectionViewModel
    {
        [Required, MaxLength(30)]
        public string Name { get; set; }

        public bool IsImageEnabled { get; set; } = false;
    }
}
