using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class CollectionQueryFormModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
