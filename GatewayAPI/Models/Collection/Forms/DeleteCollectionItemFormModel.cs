using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class DeleteCollectionItemFormModel
    {
        [Required]
        public string CollectionId { get; set; }

        [Required]
        public string CollectionItemId { get; set; }
    }
}
