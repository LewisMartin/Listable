using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class EditCollectionViewModel
    {
        public EditCollectionViewModel()
        {
            CollectionDetails = new CollectionEditor();
        }

        [Required]
        public string CollectionId { get; set; }

        [Required]
        public CollectionEditor CollectionDetails { get; set; }
    }
}
