using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CreateCollectionViewModel
    {
        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "Enable image uploads")]
        public bool IsImageEnabled { get; set; } = false;

        [Display(Name = "Grid display")]
        public bool GridDisplay { get; set; } = false;
    }
}
