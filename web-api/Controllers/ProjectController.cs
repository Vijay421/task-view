using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using WebApi.DAL;
using WebApi.Models;
using static WebApi.Controllers.UserContext;
using WebApi.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/project")]
public class ProjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly TaskViewContext _context;
    private readonly IUserContext _userContext;

    public ProjectController(ILogger<ProjectController> logger, UserManager<User> userManager, TaskViewContext context, IUserContext userContext)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
        _userContext = userContext;
    }

    [Authorize]
    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var (user, problem) = await _userContext.GetCurrentUser(User, _userManager);
        if (problem != null) return problem;

        return Results.Ok(user);
    }
}
