using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    private const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/games")
                .WithParameterValidation();

        group.MapGet("/", async (IGamesRepository repository) =>
        {
            var games = await repository.GetAllAsync();

            return Results.Ok(games.Select(g => g.AsDto()));

        });

        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game == null) return Results.NotFound();
            return Results.Ok(game.AsDto());

        }).WithName(GetGameEndpointName)
        .RequireAuthorization(Policies.ReadAccess);

        group.MapPost("/", async (IGamesRepository repository, CreateGameDto gameDto) =>
        {
            Game game = new()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri
            };

            await repository.CreateAsync(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        }).RequireAuthorization(Policies.WriteAccess);

        group.MapPut("/{id}", async (IGamesRepository repository, int id, UpdateGameDto gameDto) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = gameDto.Name;
            existingGame.Genre = gameDto.Genre;
            existingGame.Price = gameDto.Price;
            existingGame.ReleaseDate = gameDto.ReleaseDate;
            existingGame.ImageUri = gameDto.ImageUri;

            await repository.UpdateAsync(existingGame);

            return Results.NoContent();
        }).RequireAuthorization(Policies.WriteAccess);

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            var gameToDelete = await repository.GetAsync(id);

            if (gameToDelete is null) return Results.NotFound();

            await repository.DeleteAsync(id);

            return Results.NoContent();
        }).RequireAuthorization(Policies.WriteAccess);

        return group;
    }
}