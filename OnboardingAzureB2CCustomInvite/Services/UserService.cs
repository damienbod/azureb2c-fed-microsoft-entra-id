using Microsoft.Extensions.Configuration;
using OnboardingAzureB2CCustomInvite.CreateUser;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Services;

public class UserService
{
    private readonly UserContext _userContext;

    public UserService(UserContext userContext)
    {
        _userContext = userContext;
    }

    public bool IsEmailValid(string email)
    {
        if (!MailAddress.TryCreate(email, out var mailAddress))
            return false;

        // And if you want to be more strict:
        var hostParts = mailAddress.Host.Split('.');
        if (hostParts.Length == 1)
            return false; // No dot.
        if (hostParts.Any(p => p == string.Empty))
            return false; // Double dot.
        if (hostParts[^1].Length < 2)
            return false; // TLD only one letter.

        if (mailAddress.User.Contains(' '))
            return false;
        if (mailAddress.User.Split('.').Any(p => p == string.Empty))
            return false; // Double dot or dot at end of user part.

        return true;
    }

    public static string GetRandomString()
    {
        var random = $"{GenerateRandom()}{GenerateRandom()}{GenerateRandom()}{GenerateRandom()}-AC";
        return random;
    }

    private static int GenerateRandom()
    {
        return RandomNumberGenerator.GetInt32(100000000, int.MaxValue);
    }

    public async Task<UserEntity> CreateUser(UserEntity userModel)
    {
        userModel.OnboardingRegistrationCode = GetRandomString();

        await _userContext.AddAsync(userModel);

        return userModel;
    }
}