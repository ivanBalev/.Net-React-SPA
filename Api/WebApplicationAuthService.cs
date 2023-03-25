using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using CryptoHelper;
using System.Text;

public interface IWebApplicationAuthService
{
    public AuthDto GetAuthData(string id);

    public string HashPassword(string password);

    public bool VerifyPassword(string actualPassword, string hashedPassword);
}


public class WebApplicationAuthService : IWebApplicationAuthService
{
    private readonly string jwtSecret;
    private readonly int jwtLifespan;

    public WebApplicationAuthService(string jwtSecret, int jwtLifespan)
    {
        this.jwtSecret = jwtSecret;
        this.jwtLifespan = jwtLifespan;
    }
    public AuthDto GetAuthData(string id)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(jwtLifespan);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, id)
                }),
            Expires = expirationTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return new AuthDto(token, ((DateTimeOffset)expirationTime).ToUnixTimeSeconds(), id);
    }

    public string HashPassword(string password)
    {
        return Crypto.HashPassword(password);
    }

    public bool VerifyPassword(string actualPassword, string hashedPassword)
    {
        return Crypto.VerifyHashedPassword(hashedPassword, actualPassword);
    }
}