using System;
using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2C.CreateUser
{
    public class UserModelB2CIdentity
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string UserPrincipalName { get; set; } = string.Empty;

        [Required]
        public string PreferredLanguage { get; set; } = "de";

        [Required]
        public DateTimeOffset BirthDate { get; set; }
    }
}
