using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection
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
