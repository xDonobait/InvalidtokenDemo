using System.Text;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<TokenService>();

var key = Encoding.ASCII.GetBytes(TokenService.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    // Rechazar tokens invalidados
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (tokenService.IsTokenInvalidated(token))
            {
                context.Fail("Token has been invalidated.");
            }

            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
