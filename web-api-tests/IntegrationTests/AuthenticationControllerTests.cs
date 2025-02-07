using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApi;

namespace WebApiTests.IntegrationTests;

[Collection("Database")]
public class AuthenticationControllerTests : IClassFixture<ApplicationFactory<Program>>
{
    private readonly ApplicationFactory<Program> _factory;

    public AuthenticationControllerTests(ApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions());

    [Fact]
    public async Task Register_ShouldCreateAUser()
    {
        // Arrange
        var client = CreateClient();
        var credentials = new { Email = "test-user-new@test.com", Username = "test-user-new", Password = "qwerty123" };

        //Act
        var response = await client.PostAsJsonAsync("api/v1/auth/register", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldSuccessfullyLogin()
    {
        // Arrange
        var client = CreateClient();
        var credentials = new { Email = "test-user1@test.com", Password = "qwerty123" };

        //Act
        var response = await client.PostAsJsonAsync("api/v1/auth/login", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenTheUserDoesNotExist()
    {
        // Arrange
        var client = CreateClient();
        var credentials = new { Email = "none@none.com", Password = "qwerty123" };

        //Act
        var response = await client.PostAsJsonAsync("api/v1/auth/login", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ShouldSuccessfullyLogout()
    {
        // Arrange
        var client = CreateClient();
        var credentials = new { Email = "test-user-logout@test.com", Password = "qwerty123" };

        //Act
        var loginResponse = await client.PostAsJsonAsync("api/v1/auth/login", credentials);
        AuthUtil.SetAuthCookie(loginResponse, client);

        var logoutResponse = await client.PostAsync("api/v1/auth/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldDeleteTheCurrentLoggedInUser()
    {
        // Arrange
        var client = CreateClient();
        var credentials = new { Email = "test-user-delete@test.com", Password = "qwerty123" };

        // Act
        var loginResponse = await client.PostAsJsonAsync("api/v1/auth/login", credentials);
        AuthUtil.SetAuthCookie(loginResponse, client);

        var deleteResponse = await client.DeleteAsync("api/v1/auth");

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}
