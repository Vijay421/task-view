using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

using WebApi.DAL;
using WebApi.Models;

namespace WebApiTests;

public class ApplicationFactory<T> : WebApplicationFactory<T>
    where T : class
{
    // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0#customize-webapplicationfactory
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(DbContextOptions<TaskViewContext>)
                );

            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(DbConnection)
                );

            if (dbConnectionDescriptor is not null)
                services.Remove(dbConnectionDescriptor);

            // Get the test db url and use it for the integration tests.
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var testDbUrl = config["ConnectionStrings:TestDbConnection"] ?? throw new Exception("Could not find 'TestDbConnection' in appsettings.json");

                services.AddDbContext<TaskViewContext>(options =>
                    options.UseNpgsql(testDbUrl));
            }

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskViewContext>();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                context.SaveChanges();

                // Seed users.
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var tasks = new List<Task>();
                GetUsers().ForEach(data =>
                {
                    userManager.CreateAsync(data.Item1, "qwerty123").Wait();
                    userManager.AddToRoleAsync(data.Item1, data.Item2).Wait();
                });
            }
        });

        builder.UseEnvironment("Test");
    }

    private List<(User, string)> GetUsers()
    {
        return
        [
            (new User { Email = "test-user1@test.com", UserName = "test-user1", CreatedAt = DateTimeOffset.UtcNow }, "user"),
            (new User { Email = "test-user-logout@test.com", UserName = "test-user-logout", CreatedAt = DateTimeOffset.UtcNow }, "user"),
            (new User { Email = "test-user-delete@test.com", UserName = "test-user-delete", CreatedAt = DateTimeOffset.UtcNow }, "user"),
            (new User { Email = "test-admin1@test.com", UserName = "admin-user", CreatedAt = DateTimeOffset.UtcNow }, "admin"),
        ];
    }
}
