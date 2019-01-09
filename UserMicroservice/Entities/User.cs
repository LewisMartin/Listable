using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Listable.UserMicroservice.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SubjectId { get; set; }

        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
