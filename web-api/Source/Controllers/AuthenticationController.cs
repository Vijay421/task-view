using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

using WebApi.DTOs;
using WebApi.Services;
using WebApi.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;

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

        return Results.Ok(new { message = "Registered successfully!" });
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

    [Authorize]
    [HttpDelete]
    public async Task<IResult> Delete()
    {
        try
        {
            var didDelete = await _authService.Delete();

            if (!didDelete)
                return Results.Problem("Could not delete the account", statusCode: Status400BadRequest);

            await _authService.Logout();
        }
        catch(NoUserFoundException)
        {
            return Results.Problem("Account not found", statusCode: Status404NotFound);
        }

        return Results.Ok(new { message = "Your account has been deleted!" });
    }
}
