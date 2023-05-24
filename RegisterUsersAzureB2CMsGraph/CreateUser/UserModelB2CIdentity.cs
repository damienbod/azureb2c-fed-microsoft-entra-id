using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2CMsGraph.CreateUser;

public class UserModelB2CIdentity
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PreferredLanguage { get; set; } = "de";

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string GivenName { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset BirthDate { get; set; } = DateTimeOffset.UtcNow.AddYears(-30);
}
