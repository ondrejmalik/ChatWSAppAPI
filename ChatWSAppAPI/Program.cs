using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChatWSAppAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes("Jwt:Key")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

RealmConnection realmConnection = new();

app.MapGet("/all", () => { return realmConnection.SelectAllMessages(); })
    .WithName("GetAllMessages")
    .WithOpenApi()
    .RequireAuthorization();

app.MapGet("/user", (string user) => { return realmConnection.MessagesByUser(user); })
    .WithName("GetMessagesByUser")
    .WithOpenApi()
    .RequireAuthorization();

app.MapGet("/group", (string groupname, string? key) => { return realmConnection.MessagesByGroup(groupname, key); })
    .WithName("GetMessagesByGroup")
    .WithOpenApi()
    .RequireAuthorization();

app.MapGet("/word", (string word) => { return realmConnection.MessagesByWord(word); })
    .WithName("GetMessagesByWord")
    .WithOpenApi()
    .RequireAuthorization();

app.MapPost("/createToken",
        [AllowAnonymous](User user) =>
        {
            if (user.UserName == "string" && user.Password == "string")
            {
                var issuer = builder.Configuration["Jwt:Issuer"];
                var audience = builder.Configuration["Jwt:Audience"];

                var key = new byte[64];
                byte[] KeyBytes = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Audience"]);
                for (int i = 0; i < KeyBytes.Length; i++)
                {
                    key[i] = KeyBytes[i];
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);
                return Results.Ok(stringToken);
            }

            return Results.Unauthorized();
        }).WithName("CreateToken")
    .WithOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public class User
{
    public string UserName { get; set; }
    public string Password { get; set; }
}