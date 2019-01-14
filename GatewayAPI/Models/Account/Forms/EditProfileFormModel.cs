using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Account
{
    public class EditProfileFormModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
