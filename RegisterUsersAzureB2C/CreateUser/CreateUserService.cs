using Microsoft.Extensions.Configuration;
using RegisterUsersAzureB2C.CreateUser;
using RegisterUsersAzureB2C.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C;

public class CreateUserService
{
    private readonly MsGraphService _msGraphService;
    private readonly IConfiguration _configuration;

    public CreateUserService(MsGraphService msGraphService,
        IConfiguration configuration)
    {
        _msGraphService = msGraphService;
        _configuration = configuration;
    }

    public async Task<(string Upn, string Password, string Id)> CreateUserAsync(UserModelB2CTenant user)
    {
        var createdUser = await _msGraphService.CreateAzureB2CUserAsync(user);
        return createdUser;
    }

    public async Task<(string Upn, string Password, string Id)> CreateGuestUserAsync(UserModelB2CIdentity user)
    {
        var createdUser = await _msGraphService.CreateAzureB2CGuestUserAsync(user);
        return createdUser;
    }

    public async Task<(string Upn, string Password, string Id)> CreateFederatedUserAsync(UserModelB2CTenant user)
    {
        var createdUser = await _msGraphService.CreateFederatedUserAsync(user);
        return createdUser;
    }


}