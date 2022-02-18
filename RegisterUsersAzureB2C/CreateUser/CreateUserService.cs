using Microsoft.Extensions.Configuration;
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

    public async Task CreateUserAsync()
    {


    }
}