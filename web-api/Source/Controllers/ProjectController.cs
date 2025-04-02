using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.AspNetCore.Authorization;

using WebApi.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/projects")]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var projects = await _projectService.GetAll();
        if (projects.Count == 0)
            return Results.Problem("No projects found", statusCode: Status404NotFound);

        return Results.Ok(projects);
    }

   [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var project = await _projectService.Get(id);
        if (project is null)
            return Results.Problem($"No project found with id: '{id}'", statusCode: Status404NotFound);

        return Results.Ok(project);
    }

   [HttpPost]
    public async Task<IResult> Create(ProjectCreateRequest projectReq)
    {
        try
        {
            var project = await _projectService.Create(projectReq);
            return Results.Ok(project);
        }
        catch(IncorrectProjectNameException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status422UnprocessableEntity);
        }
        catch(DbDuplicateException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status409Conflict);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IResult> Update(int id, ProjectUpdateRequest projectReq)
    {
        try
        {
            var project = await _projectService.Update(id, projectReq);
            if (project is null)
                return Results.Problem($"No project with id: '{id}'", statusCode: Status404NotFound);

            return Results.Ok(project);
        }
        catch(IncorrectProjectNameException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status422UnprocessableEntity);
        }
        catch(DbDuplicateException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status409Conflict);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var didDelete = await _projectService.Delete(id);
        if (!didDelete)
            return Results.Problem($"No project with id: '{id}'", statusCode: Status404NotFound);

        return Results.Ok();
    }
}
