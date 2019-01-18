using System;

namespace GatewayAPI.Models.Account
{
    public class ProfileView
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool DisplayPrivateProfile { get; set; }
    }
}
