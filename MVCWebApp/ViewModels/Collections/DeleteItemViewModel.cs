using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class DeleteItemViewModel
    {
        [Required]
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        [Required]
        public IList<DeleteItemOption> DeleteItemOptions { get; set; }
    }

    public class DeleteItemOption
    {
        public bool IsOptionSelected { get; set; } = false;

        public string ItemId { get; set; }

        public string ItemName { get; set; } 
    }
}
