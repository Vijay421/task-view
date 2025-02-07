using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Identity;

using WebApi.Services;
using WebApi.Models;
using WebApi.DTOs;
using WebApiTests.Mocks;
using WebApi.DAL.Repositories;

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
        var mockContext = MockUtil.CreateDbWithTransaction();
        var mockUserManager = MockUtil.CreateUserManager();
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
    public async Task Register_ShouldReturnAnException_WhenGivenIncorrectFields(string errorCode)
    {
        // Arrange
        var mockContext = MockUtil.CreateDbWithTransaction();
        var mockUserManager = MockUtil.CreateUserManager();
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

            default:
                await Assert.ThrowsAsync<UnableToRegisterUserException>(() => task);
            break;
        }
    }

    [Fact]
    public async Task Register_ShouldReturnAnException_WhenTheUserRoleCouldNotBeAdded()
    {
        // Arrange
        var mockContext = MockUtil.CreateDbWithTransaction();
        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager
            .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var authService = new AuthenticationService(_mockLogger.Object, mockContext.Object, mockUserManager.Object, null!);
        var registerReq = new RegisterRequest ("test@test.com", "test", "password123" );

        // Act
        var task = authService.Register(registerReq);

        // Assert
        await Assert.ThrowsAsync<UnableToRegisterUserException>(() => task);
    }

    [Fact]
    public async Task Login_ShouldLoginCorrectly()
    {
        // Arrange
        var user = new User { CreatedAt = DateTimeOffset.UtcNow };

        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var mockUserRepo = new Mock<IUserRepository<User>>();
        mockUserRepo
            .Setup(r => r.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        var authService = new AuthenticationService(_mockLogger.Object, null!, mockUserManager.Object, mockUserRepo.Object);
        var loginReq = new LoginRequest("test@test.com", "password123" );

        // Act
        var task = authService.Login(loginReq);
        await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Login_ShouldNotLogin_WhenTheUserDoesNotExist()
    {
        // Arrange
        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => null);

        var authService = new AuthenticationService(_mockLogger.Object, null!, mockUserManager.Object, null!);
        var loginReq = new LoginRequest("test@test.com", "password123" );

        // Act
        var task = authService.Login(loginReq);

        // Assert
        await Assert.ThrowsAsync<LoginException>(() => task);
    }

    [Fact]
    public async Task Login_ShouldNotLogin_WhenTheCredentialsDoNotMatch()
    {
        // Arrange
        var user = new User { CreatedAt = DateTimeOffset.UtcNow };

        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var mockUserRepo = new Mock<IUserRepository<User>>();
        mockUserRepo
            .Setup(r => r.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        var authService = new AuthenticationService(_mockLogger.Object, null!, mockUserManager.Object, mockUserRepo.Object);
        var loginReq = new LoginRequest("test@test.com", "password123" );

        // Act
        var task = authService.Login(loginReq);

        // Assert
        await Assert.ThrowsAsync<LoginException>(() => task);
    }

    [Fact]
    public async Task Logout_ShouldBeCalled()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository<User>>();
        mockUserRepo
            .Setup(r => r.SignOutAsync())
            .Returns(Task.CompletedTask);

        var authService = new AuthenticationService(_mockLogger.Object, null!, null!, mockUserRepo.Object);

        // Act
        await authService.Logout();

        // Assert
        mockUserRepo.Verify(r => r.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldDeleteTheUser()
    {
        // Arrange
        var user = new User { CreatedAt = DateTimeOffset.UtcNow };
        var mockUserRepo = new Mock<IUserRepository<User>>();
        mockUserRepo
            .Setup(r => r.CurrentUser())
            .ReturnsAsync(user);

        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.DeleteAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var authService = new AuthenticationService(_mockLogger.Object, null!, mockUserManager.Object, mockUserRepo.Object);

        // Act
        var result = await authService.Delete();

        // Assert
        Assert.True(result, "Did not delete the user");
    }

    [Fact]
    public async Task Delete_ShouldReturnFalse_WhenTheUserWasNotDeleted()
    {
        // Arrange
        var user = new User { CreatedAt = DateTimeOffset.UtcNow };
        var mockUserRepo = new Mock<IUserRepository<User>>();
        mockUserRepo
            .Setup(r => r.CurrentUser())
            .ReturnsAsync(user);

        var mockUserManager = MockUtil.CreateUserManager();
        mockUserManager
            .Setup(u => u.DeleteAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Failed());

        var authService = new AuthenticationService(_mockLogger.Object, null!, mockUserManager.Object, mockUserRepo.Object);

        // Act
        var result = await authService.Delete();

        // Assert
        Assert.False(result, "Did delete the user, when it shouldn't");
    }
}
