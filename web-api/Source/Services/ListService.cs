using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApi.DAL;
using WebApi.DAL.Repositories;
using WebApi.Models;

namespace WebApi.Services;

public class ListService
{
    private readonly TaskViewContext _context;
    private readonly IUserRepository<User> _userRepo;

    public ListService(TaskViewContext context, IUserRepository<User> userRepo)
    {
        _context = context;
        _userRepo = userRepo;
    }

    public async Task<List<TaskList>> GetAll()
    {
        var user = await _userRepo.CurrentUser();
        var lists = await _context.Lists.Where(l => l.Project.CreatorId == user.Id).ToListAsync();

        return lists;
    }

    public async Task<ProjectResponse?> Get(int id)
    {
        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();
        var project = user.Projects.Find(p => p.Id == id);

        if (project is null) return null;

        return new ProjectResponse(project);
    }

    /// <summary>
    /// Creates the list and add it to the project with the given project id.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="listCreateRequest"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<ListResponse?> Create(int projectId, ListCreateRequest listCreateRequest)
    {
        var user = await _userRepo.CurrentUser();
        var project = await _context.Projects.FindAsync(projectId);

        if (project is null) return null;

        var collaboration = await _context.ProjectCollaborations.FindAsync(projectId, user.Id);

        var can_write = collaboration is not null && collaboration.Permission == "read_write";
        var is_creator = project.CreatorId == user.Id;

        // Only creators and people with write access are allowed to add lists.
        if (!can_write && !is_creator)
            throw new UnauthorizedException("Unable to create list due to incorrect permission");

        var list = new TaskList
        {
            Name = listCreateRequest.Name,
            Color = listCreateRequest.Color,
            IsBacklog = listCreateRequest.IsBacklog,
            CreatedAt = DateTimeOffset.UtcNow,
            ProjectId = projectId,
            Project = project,
        };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();

        return ListResponse.FormList(list);
    }

    /// <summary>
    /// Updates the project. Will return an exception when the project name contains: 'transferred'.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="projectUpdateReq"></param>
    /// <returns></returns>
    /// <exception cref="IncorrectProjectNameException"></exception>
    /// <exception cref="DbDuplicateException"></exception>
    public async Task<ProjectResponse?> Update(int id, ProjectUpdateRequest projectUpdateReq)
    {
        var projectName = projectUpdateReq.Name;
        if (projectName is not null && projectName.ToLower().Contains("transferred"))
            throw new IncorrectProjectNameException();

        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();

        var project = user.Projects.Find(p => p.Id == id);
        if (project is null) return null;

        project.Name = projectUpdateReq.Name ?? project.Name;
        project.Description = projectUpdateReq.Description ?? project.Description;

        _context.Entry(project).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            throw new DbDuplicateException($"Project with name: '{projectUpdateReq.Name}' already exists");
        }

        return new ProjectResponse(project);
    }

    public async Task<bool> Delete(int id)
    {
        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();

        var project = user.Projects.Find(p => p.Id == id);
        if (project is null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
    }
}
