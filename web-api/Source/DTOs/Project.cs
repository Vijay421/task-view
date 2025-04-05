using System.ComponentModel.DataAnnotations;

using WebApi.Models;

public record ProjectCreateRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Description { get; set; }

    public Project ToProject(User user)
    {
        var project = new Project()
        {
            Name = Name.Trim(),
            Description = Description is null ? null : Description.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatorId = user.Id,
            Creator = user,
            Lists = new List<TaskList>(),
            JoinedUsers = new List<User>(),
        };

        return project;
    }
}

public record ProjectUpdateRequest
{
    [StringLength(50, MinimumLength = 3)]
    public string? Name { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Description { get; set; }
}

// TODO: figure out if response records should have required fields.
public record ProjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string CreatorId { get; set; }
    public List<int> Lists { get; set; }
    public List<string> JoinedUsers { get; set; }

    public ProjectResponse(Project project)
    {
        Id = project.Id;
        Name = project.Name;
        Description = project.Description;
        CreatedAt = project.CreatedAt;
        DeletedAt = project.DeletedAt;
        CreatorId = project.CreatorId;
        Lists = project.Lists.Select(l => l.Id).ToList();
        JoinedUsers = project.JoinedUsers.Select(u => u.Id).ToList();
    }
}
