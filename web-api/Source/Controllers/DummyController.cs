using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApi.DAL;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/dummy")]
public class DummyController : ControllerBase
{
    private readonly ILogger<DummyController> _logger;
    private readonly TaskViewContext _context;

    public DummyController(ILogger<DummyController> logger, TaskViewContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("text")]
    public IResult GetText()
    {
        _logger.LogInformation($"called the {nameof(GetText)} endpoint");

        return Results.Ok(new { text = "Hello, World!"});
    }

    [Authorize]
    [HttpGet]
    public async Task<IResult> Get()
    {
        _logger.LogInformation($"called the {nameof(Get)} endpoint");

        var users = await _context.Users.ToListAsync();

        return Results.Ok(users);
    }
}
