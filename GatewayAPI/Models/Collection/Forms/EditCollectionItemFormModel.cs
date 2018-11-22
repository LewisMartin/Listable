using GatewayAPI.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class EditCollectionItemFormModel
    {
        [Required]
        public string CollectionId { get; set; }

        [Required]
        public string Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [FileSize(8000000)]
        [FileType("jpg,jpeg,png,gif,bmp")]
        public IFormFile ImageFile { get; set; }
    }
}
