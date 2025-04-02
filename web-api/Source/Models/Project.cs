using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

// TODO: add Link table.
public class Project
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public required string CreatorId { get; set; }
    public required User Creator { get; set; }

    public required List<TaskList> Lists { get; set; }

    public required List<User> JoinedUsers { get; set; }

    public Project()
    {
        Lists = new List<TaskList>();
        JoinedUsers = new List<User>();
    }
}
