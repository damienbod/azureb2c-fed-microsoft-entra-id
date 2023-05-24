using System.ComponentModel.DataAnnotations;

namespace RegisterUsersAzureB2CMsGraph.CreateUser;

public class UserInvite
{
    [Required]
    public string Email { get; set; } = string.Empty;
}
