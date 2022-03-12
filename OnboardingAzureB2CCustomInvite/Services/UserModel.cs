using System;
using System.ComponentModel.DataAnnotations;

namespace OnboardingAzureB2CCustomInvite.Services;

public class UserModel
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string PreferredLanguage { get; set; } = "de";

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset BirthDate { get; set; } = DateTimeOffset.UtcNow.AddYears(-30);

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    public string OnboardingRegistrationCode { get; set; } = string.Empty;

    public string AzureOid { get; set; } = string.Empty;
}