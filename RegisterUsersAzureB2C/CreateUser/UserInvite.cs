using System;
using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2C.CreateUser
{
    public class UserInvite
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
