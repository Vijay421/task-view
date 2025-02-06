using Microsoft.AspNetCore.Identity;

using WebApi.Models;
using WebApi.DTOs;
using WebApi.DAL;
using WebApi.DAL.Repositories;

namespace WebApi.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly TaskViewContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository<User> _userRepo;

    public AuthenticationService(ILogger<AuthenticationService> logger, TaskViewContext context, UserManager<User> userManager, IUserRepository<User> userRepo)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _userRepo = userRepo;
    }

    /// <summary>
    /// Tries to register the user, with the user role.
    /// </summary>
    /// <param name="registerReq"></param>
    /// <returns></returns>
    /// <exception cref="DuplicateUserNameException"></exception>
    /// <exception cref="DuplicateEmailException"></exception>
    /// <exception cref="PasswordTooShortException"></exception>
    /// <exception cref="UnableToRegisterUserException"></exception>
    public async Task Register(RegisterRequest registerReq)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var user = new User { Email = registerReq.Email, UserName = registerReq.UserName, CreatedAt = DateTimeOffset.UtcNow };
        var registerResult = await _userManager.CreateAsync(user, registerReq.Password);
        
        if (!registerResult.Succeeded)
        {
            foreach (var error in registerResult.Errors) 
            {
                if (error.Code == "DuplicateUserName")
                    throw new DuplicateUserNameException($"User name '{registerReq.UserName}' has been taken");

                if (error.Code == "DuplicateEmail")
                    throw new DuplicateEmailException($"Email address '{registerReq.Email}' has been taken");

                if (error.Code == "PasswordTooShort")
                    throw new PasswordTooShortException("Password should be at least 8 characters");
            }

            _logger.LogError("Failed to create the user, error:");
            _logger.LogError(string.Join("\n", registerResult.Errors));
            throw new UnableToRegisterUserException("An error has occurred when registering a user");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "user");
        if (!roleResult.Succeeded)
        {
            _logger.LogError("Failed to add the user role, error:");
            _logger.LogError(string.Join("\n", registerResult.Errors));
            throw new UnableToRegisterUserException("An error has occurred when registering a user");
        }

        await transaction.CommitAsync();
    }

    /// <summary>
    /// Tries to perform a login with the given credentials.
    /// </summary>
    /// <param name="loginReq"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    public async Task Login(LoginRequest loginReq)
    {
        var user = await _userManager.FindByEmailAsync(loginReq.Email);
        if (user is null)
            throw new LoginException("Invalid email or password");

        var result = await _userRepo.PasswordSignInAsync(user, loginReq.Password, isPersistent: true, lockoutOnFailure: true);
        if (!result.Succeeded)
            throw new LoginException("Invalid email or password");
    }

    /// <summary>
    /// Performs logout functionality.
    /// </summary>
    /// <returns></returns>
    public async Task Logout()
    {
        await _userRepo.SignOutAsync();
    }

    /// <summary>
    /// Attempts to delete the current logged-in user.
    /// </summary>
    /// <returns>Returns false if the user couldn't be deleted</returns>
    public async Task<bool> Delete()
    {
        var user = await _userRepo.CurrentUser();
        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to delete the user, error:");
            _logger.LogError(string.Join("\n", result.Errors));

            return false;
        }

        return true;
    }
}

public abstract class RegisterException : Exception
{
    public RegisterException(string message) : base(message) {}
}

public class DuplicateUserNameException : RegisterException
{
    public DuplicateUserNameException(string message) : base(message) {}
}

public class DuplicateEmailException : RegisterException
{
    public DuplicateEmailException(string message) : base(message) {}
}

public class PasswordTooShortException : RegisterException
{
    public PasswordTooShortException(string message) : base(message) {}
}

public class UnableToRegisterUserException : RegisterException
{
    public UnableToRegisterUserException(string message) : base(message) {}
}


public class LoginException : Exception
{
    public LoginException(string message) : base(message) {}
}
