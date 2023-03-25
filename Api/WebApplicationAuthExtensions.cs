using Microsoft.AspNetCore.Mvc;

public static class WebApplicationAuthExtensions
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/login", async (
            HttpContext httpContext,
            IUserRepository userRepo,
            IWebApplicationAuthService authService,
            [FromBody] LoginDto loginDto) =>
        {
            var user = await userRepo.GetSingle(u => u.Email == loginDto.Email);
            if (user == null)
                return Results.Problem($"No user with email {loginDto.Email} found", statusCode: 404);

            var passwordValid = authService.VerifyPassword(loginDto.Password, user.Password);
            if (!passwordValid)
                return Results.Problem("Invalid password", statusCode: 404);

            var authData = authService.GetAuthData(user.Id);
            httpContext.Response.Cookies.Append("X-Access-Token", authData.Token,
                new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.FromUnixTimeSeconds(authData.TokenExpirationTime)
                });

            return Results.Ok(authData);
        })
        .ProducesProblem(StatusCodes.Status404NotFound)
        .Produces<AuthDto>(StatusCodes.Status200OK);

        app.MapPost("/register", async (
            IUserRepository userRepo,
            IWebApplicationAuthService authService,
            [FromBody] RegisterDto registerDto
        ) =>
        {
            var emailIsUnique = await userRepo.isEmailUnique(registerDto.Email);
            if (!emailIsUnique)
                return Results.Problem("User with this email already exists", statusCode: 400);

            var usernameIsUnique = await userRepo.isUsernameUnique(registerDto.Username);
            if (!usernameIsUnique)
                return Results.Problem("User with this email already exists", statusCode: 400);


            var newUser = await userRepo.Create(registerDto.Username, registerDto.Email, authService.HashPassword(registerDto.Password));

            return Results.Created("/", authService.GetAuthData(newUser.Id));
        })
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .Produces<AuthDto>(StatusCodes.Status200OK);
    }
}