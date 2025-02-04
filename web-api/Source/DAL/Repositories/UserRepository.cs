using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using WebApi.Models;

namespace WebApi.DAL.Repositories;

public interface IUserRepository<T>
    where T : class
{
    /// <summary>
    /// Tries to find the current logged-in user.
    /// Will throw a NoUserFoundException if any claims are missing or if the user could not be found.
    /// </summary>
    /// <returns></returns>
    public Task<T> CurrentUser();
}

public class UserRepository : IUserRepository<User>
{
    private readonly ILogger<UserRepository> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(ILogger<UserRepository> logger, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<User> CurrentUser()
    {
        var claimsPrincipal = _httpContextAccessor?.HttpContext?.User;
        if (claimsPrincipal is null)
        {
            _logger.LogDebug("No user claims principal");
            throw new NoUserFoundException();
        }

        var id = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null)
        {
            _logger.LogDebug("No id claims principal");
            throw new NoUserFoundException();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            _logger.LogDebug($"No user found with id: '{id}' from claims principal");
            throw new NoUserFoundException();
        }

        return user;
    }
}

public class NoUserFoundException : Exception
{
    public NoUserFoundException() : base() {}
}
