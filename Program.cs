using System.Text.Json;
using GitHubDeveloperExplorer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/students", () =>
{
    return "Welcome to the Students API!";
});




app.MapGet("/github/{username}", async (
    string username,
    IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "GitHubDeveloperExplorer"
    );

    var response = await client.GetAsync(
        $"https://api.github.com/users/{Uri.EscapeDataString(username)}"
    );

    if (!response.IsSuccessStatusCode)
    {
        return Results.NotFound("GitHub user not found.");
    }

    var data = await response.Content.ReadAsStringAsync();

    var developer = JsonSerializer.Deserialize<Developer>(data);

    if (developer == null)
    {
        return Results.Problem("Could not process GitHub data.");
    }

    return Results.Ok(developer);
});


app.MapGet("/github/{username}/repos", async (
    string username,
    IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    client.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubDeveloperExplorer");

    var response = await client.GetAsync(
        $"https://api.github.com/users/{Uri.EscapeDataString(username)}/repos"
    );

    if (!response.IsSuccessStatusCode)
    {
        return Results.NotFound("Repositories not found.");
    }

    var data = await response.Content.ReadAsStringAsync();

    var repositories = JsonSerializer.Deserialize<List<Repository>>(data);

    return Results.Ok(repositories);
});




app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


