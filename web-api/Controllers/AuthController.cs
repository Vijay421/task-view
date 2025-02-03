using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using WebApi.DAL;
using WebApi.Models;
using WebApi.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly TaskViewContext _context;

    public AuthController(ILogger<AuthController> logger, SignInManager<User> signInManager, UserManager<User> userManager, TaskViewContext context)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterRequest registerReq)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var user = new User { Email = registerReq.Email, UserName = registerReq.UserName, CreatedAt = DateTimeOffset.UtcNow };
        var registerResult = await _userManager.CreateAsync(user, registerReq.Password);
        
        if (!registerResult.Succeeded)
        {
            foreach (var error in registerResult.Errors) 
            {
                if (error.Code == "DuplicateUserName")
                    return Results.Problem($"User name '{registerReq.UserName}' has been taken", statusCode: Status409Conflict);

                if (error.Code == "DuplicateEmail")
                    return Results.Problem($"Email address '{registerReq.Email}' has been taken", statusCode: Status409Conflict);

                if (error.Code == "PasswordTooShort")
                    return Results.Problem("Password should be at least 8 characters", statusCode: Status422UnprocessableEntity);
            }

            _logger.LogError("Uncaught error, when creating the user:");
            _logger.LogError(string.Join("\n", registerResult.Errors));
            return Results.Problem("An error has occurred when registering a user.", statusCode: Status400BadRequest);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "user");
        if (!roleResult.Succeeded)
        {
            _logger.LogError("Uncaught error, when adding the user role:");
            _logger.LogError(string.Join("\n", registerResult.Errors));
            return Results.Problem("An error has occurred when registering a user.", statusCode: Status400BadRequest);
        }

        await transaction.CommitAsync();

        return Results.Ok(new { message = "User registered successfully!" });
    }

    [HttpPost("login")]
    public async Task<IResult> Login(LoginRequest loginReq)
    {
        var user = await _userManager.FindByEmailAsync(loginReq.Email);
        if (user == null)
            return Results.Problem("Invalid email or password", statusCode: Status401Unauthorized);

        var result = await _signInManager.PasswordSignInAsync(user, loginReq.Password, isPersistent: true, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Results.Problem("Invalid email or password", statusCode: Status401Unauthorized);

        return Results.Ok(new { message = "Login successful!" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return Ok(new { message = "Logged out successfully!" });
    }
}
