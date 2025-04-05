using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

// TODO: add Link table.
public class Project
{
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public required string CreatorId { get; set; }
    public required User Creator { get; set; }

    public List<TaskList> Lists { get; set; } = new();

    public List<User> JoinedUsers { get; set; } = new(); // TODO: change to relation with ProjectCollaboration.
}
