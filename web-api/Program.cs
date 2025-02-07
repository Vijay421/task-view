using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebApi.DAL;
using WebApi.DAL.Repositories;
using WebApi.Middlewares;
using WebApi.Models;
using WebApi.Services;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Do not register the db connection when testing!
        if (!builder.Environment.IsEnvironment("Test"))
        {
            builder.Services.AddDbContext<TaskViewContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        ConfigureProblemDetails(builder);
        ConfigureIdentity(builder);

        builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddScoped<ProjectService>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    /// <summary>
    /// Adds the URI path to Problem responses.
    /// </summary>
    /// <param name="builder"></param>
    private static void ConfigureProblemDetails(WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context => 
            {
                var request = context.HttpContext.Request;
                context.ProblemDetails.Instance = $"{request.Method} {request.Path}";
            };
        });
    }

    /// <summary>
    /// Configure identity according to the owasp best practices, and set the auth cookie.
    /// </summary>
    /// <param name="builder"></param>
    private static void ConfigureIdentity(WebApplicationBuilder builder)
    {
        // Store keys with EntityFramework: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-9.0#persistkeystodbcontext
        builder.Services.AddDataProtection();

        // Add Identity's authorization: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-9.0#add-identity-services-to-the-container
        builder.Services.AddAuthorization();

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<TaskViewContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // OWASP password best practices: https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html#implement-proper-password-strength-controls
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;

            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            options.User.RequireUniqueEmail = true;
        });

        // Enable cookie authentication.
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/api/v1/auth/login";
            options.LogoutPath = "/api/v1/auth/logout";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });
    }
}
