using System.ComponentModel.DataAnnotations;

namespace Listable.CollectionMicroservice.DTO
{
    public class CollectionQuery
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
