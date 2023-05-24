using System.ComponentModel.DataAnnotations;

namespace OnboardingAzureB2CCustomInvite.Services;

public class Profile
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
}
