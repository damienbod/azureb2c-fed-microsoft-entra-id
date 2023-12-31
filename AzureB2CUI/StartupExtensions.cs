using AzureB2CUI.Authz;
using AzureB2CUI.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace AzureB2CUI;

internal static class StartupExtensions
{
    private static IWebHostEnvironment? _env;
    private static IServiceProvider? _applicationServices;
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        _env = builder.Environment;

        services.AddTransient<AdminApiService>();
        services.AddTransient<UserApiService>();
        services.AddScoped<MsGraphService>();
        services.AddScoped<MsGraphClaimsTransformation>();
        services.AddHttpClient();

        services.AddOptions();

        string[]? initialScopes = configuration.GetValue<string>("UserApiOne:ScopeForAccessToken")?.Split(' ');

        services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAdB2C")
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddInMemoryTokenCaches();

        services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = async context =>
            {
                if (_applicationServices != null && context.Principal != null)
                {
                    using var scope = _applicationServices.CreateScope();
                    context.Principal = await scope.ServiceProvider
                        .GetRequiredService<MsGraphClaimsTransformation>()
                        .TransformAsync(context.Principal);
                }
            };
        });

        services.AddControllers();

        services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddMicrosoftIdentityUI();

        services.AddSingleton<IAuthorizationHandler, IsAdminHandlerUsingAzureGroups>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdminPolicy", policy =>
            {
                policy.Requirements.Add(new IsAdminRequirement());
            });
        });
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        _applicationServices = app.Services;

        IdentityModelEventSource.ShowPII = true;

        app.UseSerilogRequestLogging();

        if (_env!.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        return app;
    }
}