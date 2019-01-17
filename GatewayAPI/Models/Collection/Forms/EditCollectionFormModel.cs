using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class EditCollectionFormModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool PrivateMode { get; set; }

        [Required]
        public bool GridDisplay { get; set; }
    }
}
