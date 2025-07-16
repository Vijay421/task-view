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

    [HttpGet]
    public IResult Get()
    {
        _logger.LogInformation($"called the {nameof(Get)} endpoint");

        return Results.Ok(new { text = "Hello, World!"});
    }

    [Authorize]
    [HttpGet]
    public IResult GetSecure()
    {
        _logger.LogInformation($"called the {nameof(GetSecure)} endpoint");

        return Results.Ok(new { text = "Hello, World! From a secure route."});
    }
}
