using System;
using System.ComponentModel.DataAnnotations;

namespace OnboardingAzureB2CCustomInvite.CreateUser
{
    public class UserInvite
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
