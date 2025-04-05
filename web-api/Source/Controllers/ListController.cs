using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.AspNetCore.Authorization;

using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1")]
public class ListController : ControllerBase
{
    private readonly ListService _listService;

    public ListController(ListService listService)
    {
        _listService = listService;
    }

    [HttpGet("lists")]
    public async Task<IResult> GetAll()
    {
        var lists = await _listService.GetAll();
        if (lists.Count == 0)
            return Results.Problem("No lists found", statusCode: Status404NotFound);

        return Results.Ok(lists);
    }

    [HttpPost("projects/{projectId}/lists")]
    public async Task<IResult> Create(int projectId, ListCreateRequest listReq)
    {
        try
        {
            var list = await _listService.Create(projectId, listReq);

            if (list is null)
                return Results.Problem("Could not find the project", statusCode: Status404NotFound);

            return Results.Ok(list);
        }
        catch(UnauthorizedException ex)
        {
            return Results.Problem(ex.Message, statusCode: Status401Unauthorized);
        }
    }
}
