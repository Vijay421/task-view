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

    public async Task<List<Project>> GetAll()
    {
        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();

        return user.Projects;
    }

    public async Task<Project?> Get(int id)
    {
        var user = await _userRepo.CurrentUser();
        await _context.Entry(user).Collection(u => u.Projects).LoadAsync();
        var project = user.Projects.Find(p => p.Id == id);

        return project;
    }

    public async Task<Project> Create(ProjectCreateRequest projectCreateReq)
    {
        var user = await _userRepo.CurrentUser();
        var project = projectCreateReq.ToProject(user);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return project;
    }
}
