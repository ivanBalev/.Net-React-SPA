using System.ComponentModel.DataAnnotations;

public record RegisterDto(
    [property: Required][property: StringLength(60, MinimumLength = 2)] string Username,
    [property: Required][property: EmailAddress] string Email,
    [property: Required][property: StringLength(60, MinimumLength = 3)] string Password);