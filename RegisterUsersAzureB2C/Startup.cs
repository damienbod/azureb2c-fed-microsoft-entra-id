using RegisterUsersAzureB2C.Authz;
using RegisterUsersAzureB2C.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;

namespace RegisterUsersAzureB2C;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    protected IServiceProvider ApplicationServices { get; set; } = null;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<MsGraphService>();
        services.AddScoped<MsGraphClaimsTransformation>();
        services.AddHttpClient();

        services.AddOptions();

        services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAdB2C")
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = async context =>
            {
                if (ApplicationServices != null && context.Principal != null)
                {
                    using var scope = ApplicationServices.CreateScope();
                    context.Principal = await scope.ServiceProvider
                        .GetRequiredService<MsGraphClaimsTransformation>()
                        .TransformAsync(context.Principal);
                }
            };
        });

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ApplicationServices = app.ApplicationServices;

        if (env.IsDevelopment())
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}