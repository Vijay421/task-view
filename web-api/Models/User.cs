using Microsoft.AspNetCore.Identity;

namespace WebApi.Models;

public class User : IdentityUser
{
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
