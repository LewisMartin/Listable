using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class EditItemViewModel
    {
        public EditItemViewModel()
        {
            ItemDetails = new ItemEditor();
        }

        public string CollectionName { get; set; }

        public string ItemId { get; set; }

        [Required]
        public ItemEditor ItemDetails { get; set; }
    }
}
