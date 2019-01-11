using System.ComponentModel.DataAnnotations;

namespace Listable.UserMicroservice.DTO
{
    public class UserDetails
    {
        public int Id { get; set; }

        public string SubjectId { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
