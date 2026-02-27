using System.Text.Json;
using GoFish.Application.Contracts;
using GoFish.Application.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GameControllerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
var logger = app.Logger;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webRootPath = ResolveWebRoot(builder.Configuration["GOFISH_WEB_ROOT"], app.Environment.ContentRootPath);
if (webRootPath is not null)
{
    var provider = new PhysicalFileProvider(webRootPath);
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = provider,
        RequestPath = string.Empty,
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = provider,
        RequestPath = string.Empty,
    });

    logger.LogInformation("Serving web dashboard from {Path}", webRootPath);
}
else
{
    logger.LogWarning("Web directory not found. API will run without dashboard assets.");
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "gofish-api",
    timestampUtc = DateTimeOffset.UtcNow,
}));

app.MapGet("/api/ranks", (GameControllerService controller) => Results.Ok(new
{
    ranks = controller.GetSupportedRanks(),
}));

app.MapGet("/api/game/state", (GameControllerService controller) => Results.Ok(controller.GetState()));

app.MapGet("/api/game/metrics", (GameControllerService controller) => Results.Ok(controller.GetMetrics()));

app.MapPost("/api/game/reset", (ResetRequestDto? request, GameControllerService controller) =>
{
    var state = controller.ResetGame(request?.Seed);
    return Results.Ok(state);
});

app.MapPost("/api/game/player/ask", (AskRequestDto request, GameControllerService controller) =>
{
    var outcome = controller.AskPlayer(request.Rank);
    return outcome.Accepted ? Results.Ok(outcome) : Results.BadRequest(outcome);
});

app.MapFallback((HttpContext context) =>
{
    if (context.Request.Path.StartsWithSegments("/api") ||
        context.Request.Path.StartsWithSegments("/health") ||
        context.Request.Path.StartsWithSegments("/swagger"))
    {
        return Results.NotFound();
    }

    if (webRootPath is null)
    {
        return Results.NotFound();
    }

    var indexPath = Path.Combine(webRootPath, "index.html");
    return File.Exists(indexPath)
        ? Results.File(indexPath, "text/html")
        : Results.NotFound();
});

app.Run();

static string? ResolveWebRoot(string? configuredPath, string contentRoot)
{
    var candidates = new List<string>();

    if (!string.IsNullOrWhiteSpace(configuredPath))
    {
        candidates.Add(Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(Directory.GetCurrentDirectory(), configuredPath));
    }

    candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "web"));
    candidates.Add(Path.Combine(contentRoot, "..", "..", "web"));

    foreach (var candidate in candidates.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase))
    {
        if (Directory.Exists(candidate))
        {
            return candidate;
        }
    }

    return null;
}
