namespace OnboardingAzureB2CCustomInvite.Services;

public class UserEntity
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "de";
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTimeOffset BirthDate { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = string.Empty;
    public string OnboardingRegistrationCode { get; set; } = string.Empty;
    public string AzureOid { get; set; } = string.Empty;
}
