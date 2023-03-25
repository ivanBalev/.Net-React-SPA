using System.ComponentModel.DataAnnotations;

public record LoginDto(
[property: Required][property: EmailAddress] string Email,
[property: Required] string Password);
