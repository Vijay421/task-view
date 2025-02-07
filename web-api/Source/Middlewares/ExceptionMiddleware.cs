using static Microsoft.AspNetCore.Http.StatusCodes;

using WebApi.DAL.Repositories;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _request;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate request)
    {
        _request = request;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch(DataException ex)
        {
            _logger.LogError("A DataException has occurred:");
            _logger.LogError(ex.Message);
        
            context.Response.StatusCode = Status500InternalServerError;
            var problem = Results.Problem("A server error has occurred", statusCode: Status500InternalServerError);
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch(NoUserFoundException)
        {
            context.Response.StatusCode = Status401Unauthorized;
            var request = context.Request;

            var problemDetails = new ProblemDetails
            {
                Status = Status401Unauthorized,
                Detail = "Incorrect user",
                Instance = $"{request.Method} {request.Path}"
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
