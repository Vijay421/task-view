using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using static Microsoft.AspNetCore.Http.StatusCodes;
using WebApi.Models;

namespace WebApi.Controllers;

public interface IUserContext
{
    /// <summary>
    /// Tries to find the current logged-in user.
    /// Will return a unauthorized problem when the user was not found.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="_userManager"></param>
    /// <returns></returns>
    public Task<(User?, IResult?)> GetCurrentUser(ClaimsPrincipal claimsPrincipal, UserManager<User> _userManager);
}

/// <summary>
/// A utility class to retrieve the current user.
/// </summary>
public class UserContext : IUserContext
{
    public async Task<(User?, IResult?)> GetCurrentUser(ClaimsPrincipal claimsPrincipal, UserManager<User> _userManager)
    {
        var notFound = Results.Problem("Incorrect user", statusCode: Status401Unauthorized);

        var id = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null) return (null, notFound);

        var user = await _userManager.FindByIdAsync(id);
        if (id == null) return (null, notFound);

        return (user, null);
    }
}
