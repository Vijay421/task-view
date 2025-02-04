using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs;

public record RegisterRequest
(
    [Required]
    [EmailAddress]
    [StringLength(254, MinimumLength = 5)]
    string Email,

    [Required]
    [StringLength(50, MinimumLength = 3)]
    string UserName,

    [Required]
    [StringLength(255, MinimumLength = 5)]
    string Password
);

public record LoginRequest
(
    [Required]
    [EmailAddress]
    [StringLength(254, MinimumLength = 5)]
    string Email,

    [Required]
    [StringLength(255, MinimumLength = 5)]
    string Password
);
