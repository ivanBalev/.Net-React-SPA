using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
// Scoped -> new instance for each request
builder.Services.AddDbContext<HouseDbContext>(o =>
    // Disable each entity instance for property changes ->
    // We don't need it here & this way we get better performance
    o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecretKey"))
            )
        };

        if (options.Events == null)
        {
            options.Events = new JwtBearerEvents();
        }

        // Attach access token from cookie to context directly
        options.Events.OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
            }

            return Task.CompletedTask;
        };
    });



builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();

builder.Services.AddSingleton<IWebApplicationAuthService>(
    new WebApplicationAuthService(
        builder.Configuration.GetValue<string>("JWT:SecretKey"),
        builder.Configuration.GetValue<int>("JWT:Lifespan")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(p => p.WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.UseHttpsRedirection();

app.MapHouseEndpoints();
app.MapBidEndpoints();
app.MapAuthEndpoints();
app.UseAuthentication();

app.Run();
