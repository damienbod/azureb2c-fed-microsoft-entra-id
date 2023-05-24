﻿using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2CMsGraph.CreateUser;

public class UserModelB2CTenant
{
    [Required]
    public string UserPrincipalName { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string GivenName { get; set; } = string.Empty;

    [Required]
    public string PreferredLanguage { get; set; } = "de";

    [Required]
    public DateTimeOffset BirthDate { get; set; } = DateTimeOffset.UtcNow.AddYears(-30);





}
