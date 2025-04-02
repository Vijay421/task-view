using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models;

public class TaskList
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    public required int Color { get; set; }

    [Required]
    public required bool IsBacklog { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public int ProjectId { get; set; }
    public required Project Project { get; set; }

    public required List<TaskItem> Items { get; set; }
}
