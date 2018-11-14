using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class DeleteCollectionModel
    {
        [Required]
        public string SelectedCollectionId { get; set; }
    }
}
