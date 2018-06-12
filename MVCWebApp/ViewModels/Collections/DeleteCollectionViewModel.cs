using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class DeleteCollectionViewModel
    {
        [Required, Display(Name = "Collection:")]
        public string SelectedCollection { get; set; }

        public List<SelectListItem> Collections { get; set; }
    }
}
