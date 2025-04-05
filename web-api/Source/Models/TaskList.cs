using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class TaskList
{
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    public required int Color { get; set; }

    // TODO: figure out a different way of specifying the role (type) of a list (e.g., backlog, todo-lists and done-lists).
    public required bool IsBacklog { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public required int ProjectId { get; set; }

    public required Project Project { get; set; }

    public List<TaskItem> Items { get; set; } = new();
}
