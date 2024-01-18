using BlazorApp2.Shared;
using ChatWSAppAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

RealmConnection realmConnection = new();

app.MapGet("/all", () =>
    {
        return realmConnection.SelectAllMessages();
    })
    .WithName("GetAllMessages")
    .WithOpenApi();

app.MapGet("/user", (string user) =>
    {
        return realmConnection.MessagesByUser(user);
    })
    .WithName("GetMessagesByUser")
    .WithOpenApi();
app.MapGet("/group", (string groupname,string? key) =>
    {
        return realmConnection.MessagesByGroup(groupname,key);
    })
    .WithName("GetMessagesByGroup")
    .WithOpenApi();

app.MapGet("/word", (string word) =>
    {
        return realmConnection.MessagesByWord(word);
    })
    .WithName("GetMessagesByWord")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}