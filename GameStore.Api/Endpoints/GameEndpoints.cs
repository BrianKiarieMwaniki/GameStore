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

        group.MapGet("/", (IGamesRepository repository) => repository.GetAll().Select(g => g.AsDto()));

        group.MapGet("/{id}", (IGamesRepository repository, int id) =>
        {
            Game? game = repository.Get(id);

            if (game == null) return Results.NotFound();
            return Results.Ok(game.AsDto());

        }).WithName(GetGameEndpointName);

        group.MapPost("/", (IGamesRepository repository, CreateGameDto gameDto) =>
        {
            Game game = new ()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri
            };

            repository.Create(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

        group.MapPut("/{id}", (IGamesRepository repository, int id, UpdateGameDto gameDto) =>
        {
            Game? existingGame = repository.Get(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = gameDto.Name;
            existingGame.Genre = gameDto.Genre;
            existingGame.Price = gameDto.Price;
            existingGame.ReleaseDate = gameDto.ReleaseDate;
            existingGame.ImageUri = gameDto.ImageUri;

            repository.Update(existingGame);

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (IGamesRepository repository, int id) =>
        {
            var gameToDelete = repository.Get(id);

            if (gameToDelete is null) return Results.NotFound();

            repository.Delete(id);

            return Results.NoContent();
        });

        return group;
    }
}