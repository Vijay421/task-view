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
        var project = new Project
        {
            Name = Name.Trim(),
            Description = Description is null ? null : Description.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = user.Id,
            User = user,
        };

        return project;
    }
}
