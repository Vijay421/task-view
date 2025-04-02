using Microsoft.EntityFrameworkCore;

using WebApi.DAL;
using WebApi.DAL.Repositories;
using WebApi.Models;

namespace WebApi.Services;

public class ProjectService
{
    private readonly TaskViewContext _context;
    private readonly IUserRepository<User> _userRepo;

    public ProjectService(TaskViewContext context, IUserRepository<User> userRepo)
    {
        _context = context;
        _userRepo = userRepo;
    }

    public async Task<List<ProjectResponse>> GetAll()
    {
        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();

        var projects = user.Projects.Select(p => new ProjectResponse(p)).ToList();

        return projects;
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
    /// Creates the project. Will return an exception when the project name contains: 'transferred'.
    /// </summary>
    /// <param name="projectCreateReq"></param>
    /// <returns></returns>
    /// <exception cref="IncorrectProjectNameException"></exception>
    public async Task<ProjectResponse> Create(ProjectCreateRequest projectCreateReq)
    {
        var projectName = projectCreateReq.Name.ToLower();
        if (projectName.Contains("transferred"))
            throw new IncorrectProjectNameException();

        var user = await _userRepo.CurrentUser();
        var project = projectCreateReq.ToProject(user);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return new ProjectResponse(project);
    }

    /// <summary>
    /// Updates the project. Will return an exception when the project name contains: 'transferred'.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="projectUpdateReq"></param>
    /// <returns></returns>
    /// <exception cref="IncorrectProjectNameException"></exception>
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
        await _context.SaveChangesAsync();

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

// Project must not contain the word 'transferred'
// because this word is used the indicate whether
// the ownership of a project was changes.
public class IncorrectProjectNameException : Exception
{
    public IncorrectProjectNameException() : base("Project name must not contain the word: 'transferred'") {}

    public IncorrectProjectNameException(string message) : base(message) {}
}
