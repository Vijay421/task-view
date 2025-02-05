using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Identity;

using WebApi.Services;
using WebApi.Models;
using WebApi.DTOs;

namespace WebApiTests.UnitTests;

public class AuthenticationServiceTests
{
    private readonly Mock<ILogger<AuthenticationService>> _mockLogger;

    public AuthenticationServiceTests()
    {
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
    }

    [Fact]
    public async Task Register_ShouldCreateAUser()
    {
        // Arrange
        var mockContext = Mocks.CreateDbWithTransaction();
        var mockUserManager = Mocks.CreateUserManager();
        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager
            .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var authService = new AuthenticationService(_mockLogger.Object, mockContext.Object, mockUserManager.Object, null!);
        var registerReq = new RegisterRequest ("test@test.com", "test", "password123" );

        // Act
        var task = authService.Register(registerReq);
        await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("DuplicateUserName")]
    [InlineData("DuplicateEmail")]
    [InlineData("PasswordTooShort")]
    [InlineData("[Unexpected error code]")]
    public async Task Register_ShouldReturnAnException_WhenThereIsAnErrorCode(string errorCode)
    {
        // Arrange
        var mockContext = Mocks.CreateDbWithTransaction();
        var mockUserManager = Mocks.CreateUserManager();
        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError{ Code = errorCode }));

        var authService = new AuthenticationService(_mockLogger.Object, mockContext.Object, mockUserManager.Object, null!);
        var registerReq = new RegisterRequest ("test@test.com", "test", "password123" );

        // Act
        var task = authService.Register(registerReq);

        // Assert
        switch (errorCode)
        {
            case "DuplicateUserName":
                await Assert.ThrowsAsync<DuplicateUserNameException>(() => task);
            break;

            case "DuplicateEmail":
                await Assert.ThrowsAsync<DuplicateEmailException>(() => task);
            break;

            case "PasswordTooShort":
                await Assert.ThrowsAsync<PasswordTooShortException>(() => task);
            break;

            case "[Unexpected error code]":
                await Assert.ThrowsAsync<UnableToRegisterUserException>(() => task);
            break;

            default:
                throw new Exception($"No assert case for error code: '{errorCode}'");
        }
    }

    [Fact]
    public async Task Register_ShouldReturnAnException_WhenTheUserRoleCouldNotBeAdded()
    {
        // Arrange
        var mockContext = Mocks.CreateDbWithTransaction();
        var mockUserManager = Mocks.CreateUserManager();
        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager
            .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError{ Code = "Error" }));

        var authService = new AuthenticationService(_mockLogger.Object, mockContext.Object, mockUserManager.Object, null!);
        var registerReq = new RegisterRequest ("test@test.com", "test", "password123" );

        // Act
        var task = authService.Register(registerReq);

        // Assert
        await Assert.ThrowsAsync<UnableToRegisterUserException>(() => task);
    }
}
