using System.ComponentModel.DataAnnotations;

using WebApi.Models;

public record ListCreateRequest
{
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    public required int Color { get; set; }

    public required bool IsBacklog { get; set; }
}

public record ListResponse
{
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    public required int Color { get; set; }

    public required bool IsBacklog { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public required int ProjectId { get; set; }

    public List<TaskItem> Items { get; set; } = new();

    public static ListResponse FormList(TaskList list)
    {
        return new ListResponse
        {
            Id = list.Id,
            Name = list.Name,
            Color = list.Color,
            IsBacklog = list.IsBacklog,
            CreatedAt = list.CreatedAt,
            DeletedAt = list.DeletedAt,
            ProjectId = list.ProjectId,
            Items = list.Items,
        };
    }
}