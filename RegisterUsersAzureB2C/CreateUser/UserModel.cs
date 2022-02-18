using System;
using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2C.CreateUser
{
    public class UserModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset BirthDate { get; set; }

        [Required]
        public string PreferredLanguage { get; set; } = "de";

        [Required]
        public string UserPrincipalName { get; set; } = string.Empty;

    }
}
