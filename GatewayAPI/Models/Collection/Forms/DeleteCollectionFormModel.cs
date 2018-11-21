using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class DeleteCollectionFormModel
    {
        [Required]
        public string SelectedCollectionId { get; set; }
    }
}
