using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Models;

public class User : IdentityUser
{
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public List<Project> Projects { get; set; }

    [JsonIgnore]
    public List<Project> JoinedProjects { get; set; }

    public User()
    {
        Projects = new List<Project>();
        JoinedProjects = new List<Project>();
    }
}
