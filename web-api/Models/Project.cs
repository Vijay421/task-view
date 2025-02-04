using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models;

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

    public required string UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}
