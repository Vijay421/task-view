using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

// TODO: add assigned users pivot table.
public class TaskItem
{
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public required string Title { get; set; }

    [StringLength(8, MinimumLength = 1)]
    public string? ShortName { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Description { get; set; }

    public required bool IsRepeating { get; set; }

    public required bool IsDone { get; set; }

    [StringLength(500, MinimumLength = 3)]
    public string? Result { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public string? CreatorId { get; set; }
    public User? Creator { get; set; }

    public required int ListId { get; set; }
    public required TaskList List { get; set; }

    public int? SuperItemId { get; set; }
    public TaskItem? SuperItem { get; set; }

    public List<TaskItem> SubItems { get; set; } = new();
}
