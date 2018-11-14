using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class CreateCollectionModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool ImageEnabled { get; set; }

        [Required]
        public bool GridDisplay { get; set; }
    }
}
