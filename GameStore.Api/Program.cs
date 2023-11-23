using GameStore.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

const string GetGameEndpointName = "GetGame";

List<Game> games = new List<Game>{
    new Game() {Id=1, Name = "Super Mario", Price = 19.99M, Genre = "Family", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(1991, 2, 1)},
    new Game() {Id=2, Name = "Street Fighter II", Price = 29.99M, Genre = "Action", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 12, 10)},
    new Game() {Id=3, Name = "NFS:Most Wanterd", Price = 10.99M, Genre = "Racing", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 4, 15)},
    new Game() {Id=4, Name = "HitMan: Blood Money", Price = 7.50M, Genre = "Action", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 2, 17)},
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var group = app.MapGroup("/api/games");

group.MapGet("/", () => games);

group.MapGet("/{id}", (int id) =>
{
    Game? game = games.Find(g => g.Id == id);

    if (game == null) return Results.NotFound();

    return Results.Ok(game);
}).WithName(GetGameEndpointName);

group.MapPost("/", (Game game) =>
{
    game.Id = games.Max(game => game.Id) + 1;

    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
});

group.MapPut("/{id}", (int id, Game game) =>
{
    Game? existingGame = games.Find(g => g.Id == id);

    if (existingGame is null)
    {
        return Results.NotFound();
    }

    existingGame.Name = game.Name;
    existingGame.Genre = game.Genre;
    existingGame.Price = game.Price;
    existingGame.ReleaseDate = game.ReleaseDate;
    existingGame.ImageUri = game.ImageUri;

    return Results.NoContent();
});

group.MapDelete("/{id}", (int id) =>
{
    var gameToDelete = games.Find(g => g.Id == id);

    if (gameToDelete is null) return Results.NotFound();

    games.Remove(gameToDelete);

    return Results.NoContent();
});

app.Run();
