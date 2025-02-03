using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Project
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 3)]
    public required string Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }
}
