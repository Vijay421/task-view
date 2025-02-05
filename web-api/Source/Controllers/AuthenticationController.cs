using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

using WebApi.DTOs;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthenticationController(AuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterRequest registerReq)
    {
        try
        {
            await _authService.Register(registerReq);
        }
        catch (RegisterException ex)
        {
            switch (ex)
            {
                case DuplicateUserNameException:
                case DuplicateEmailException:
                    return Results.Problem(ex.Message, statusCode: Status409Conflict);

                case PasswordTooShortException:
                    return Results.Problem(ex.Message, statusCode: Status422UnprocessableEntity);

                case UnableToRegisterUserException:
                    return Results.Problem(ex.Message, statusCode: Status400BadRequest);
            }
        }

        return Results.Ok(new { message = "User registered successfully!" });
    }

    [HttpPost("login")]
    public async Task<IResult> Login(LoginRequest loginReq)
    {
        try
        {
            await _authService.Login(loginReq);
        }
        catch (LoginException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status401Unauthorized);
        }

        return Results.Ok(new { message = "Login successful!" });
    }

    [HttpPost("logout")]
    public async Task<IResult> Logout()
    {
        await _authService.Logout();

        return Results.Ok(new { message = "Logged out successfully!" });
    }
}
