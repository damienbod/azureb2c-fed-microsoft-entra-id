using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Cryptography;

namespace OnboardingAzureB2CCustomInvite.Services;

public class UserService
{
    private readonly UserContext _userContext;
    private readonly MsGraphEmailService _msGraphEmailService;
    private readonly EmailService _emailService;

    public UserService(MsGraphEmailService msGraphEmailService,
       UserContext userContext, EmailService emailService)
    {
        _msGraphEmailService = msGraphEmailService;
        _userContext = userContext;
        _emailService = emailService;
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

    public async Task<UserEntity> GetUserById(int id)
    {
        return await _userContext.Users.FirstAsync(
            u => u.Id == id);
    }

    public async Task<UserEntity> UpdateUser(UserModel userModel, int id)
    {
        var user = await GetUserById(id);

        user.Email = userModel.Email;
        user.FirstName = userModel.FirstName;
        user.Surname = userModel.Surname;
        user.BirthDate = userModel.BirthDate;
        user.DisplayName = userModel.DisplayName;
        user.PreferredLanguage = userModel.PreferredLanguage;
        user.IsActive = userModel.IsActive;

        await _userContext.SaveChangesAsync();

        return user;
    }

    public async Task<int> ConnectUserIfExistsAsync(string onboardingRegistrationCode, string oid, bool isActive, string? email)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(
            u => u.OnboardingRegistrationCode == onboardingRegistrationCode);

        if (user == null)
            return 0;

        user.AzureOid = oid;

        if (email != null)
            user.Email = email;

        user.IsActive = isActive;
        _userContext.Users.Update(user);
        await _userContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<UserEntity?> FindUserWithOid(string oid)
    {
        return await _userContext.Users.FirstOrDefaultAsync(
            u => u.AzureOid == oid);
    }

    public async Task UpdateCreateProfile(UserEntity userEntity)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(
             u => u.AzureOid == userEntity.AzureOid);

        if (user == null)
        {
            await _userContext.AddAsync(userEntity);
        }
        else
        {
            _userContext.Users.Update(user);
        }

        await _userContext.SaveChangesAsync();
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
        await _userContext.SaveChangesAsync();

        return userModel;
    }

    public async Task SendEmailInvite(UserEntity user, HostString host, bool updateCode)
    {
        if (updateCode)
        {
            user.OnboardingRegistrationCode = GetRandomString();
            await _userContext.SaveChangesAsync();
        }

        var accountUrl = $"https://{host}/ConnectAccount?code={user.OnboardingRegistrationCode}";
        var header = $"{user.FirstName} {user.Surname} you are invited to signup";
        var introText = "You have been invite to join the MyApp services. You can register and sign up here";
        var endText = "Best regards, your MyApp support";
        var body = $"Dear {user.FirstName} {user.Surname} \n\n{introText} \n\n{accountUrl} \n\n{endText}";
        var message = _emailService.CreateStandardEmail(user.Email, header, body);

        await _msGraphEmailService.SendEmailAsync(message);
    }
}