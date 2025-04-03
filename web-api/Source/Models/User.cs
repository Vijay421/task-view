using Microsoft.AspNetCore.Identity;

namespace WebApi.Models;

public class User : IdentityUser
{
    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public List<Project> Projects { get; set; } = new();

    public List<Project> JoinedProjects { get; set; } = new();
}
