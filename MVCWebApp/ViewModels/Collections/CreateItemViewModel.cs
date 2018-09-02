using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CreateItemViewModel
    {
        public CreateItemViewModel()
        {
            ItemDetails = new ItemEditor();
        }

        public string CollectionName { get; set; }

        [Required]
        public ItemEditor ItemDetails { get; set; }
    }
}
