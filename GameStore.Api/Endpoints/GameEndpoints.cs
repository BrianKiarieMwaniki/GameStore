using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    private const string GetGameV1EndpointName = "GetGameV1";
    private const string GetGameV2EndpointName = "GetGameV2";

    public static RouteGroupBuilder MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                            .MapGroup("/api/games")
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .WithParameterValidation()
                            .WithOpenApi()
                            .WithTags("Games");

        group.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory,
                                [AsParameters] GetGamesDtoV1 request, HttpContext context) =>
        {
            var games = await repository.GetAllAsync(request.PageNumber, request.PageSize, request.Filter);

            var totalCount = await repository.CountAsync(request.Filter);

            context.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok(games.Select(g => g.AsDtoV1()));

        })
        .MapToApiVersion(1.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available gams and allows pagination and filtering");

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV1>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game == null) return TypedResults.NotFound();
            return TypedResults.Ok(game.AsDtoV1());

        })
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Gets game by id")
        .WithDescription("Get the game that has the specified id");

        #region V2 endpoints
        group.MapGet("/", async (IGamesRepository repository, [AsParameters] GetGamesDtoV2 request, HttpContext context) =>
        {
            var games = await repository.GetAllAsync(request.PageNumber, request.PageSize, request.Filter);

            var totalCount = await repository.CountAsync(request.Filter);

            context.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok(games.Select(g => g.AsDtoV2()));

        })
        .MapToApiVersion(2.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available gams and allows pagination and filtering");

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV2>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game == null) return TypedResults.NotFound();
            return TypedResults.Ok(game.AsDtoV2());

        })
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0)
        .WithSummary("Gets game by id")
        .WithDescription("Get the game that has the specified id");

        #endregion

        group.MapPost("/", async Task<CreatedAtRoute<GameDtoV1>> (IGamesRepository repository, CreateGameDto gameDto) =>
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

            return TypedResults.CreatedAtRoute(game.AsDtoV1(), GetGameV1EndpointName, new { id = game.Id });
        })
        .MapToApiVersion(1.0)
        .RequireAuthorization(Policies.WriteAccess)
        .WithSummary("Creates a new game")
        .WithDescription("Creates a new game with the specified properties");

        group.MapPut("/{id}", async Task<Results<NotFound, NoContent>> (IGamesRepository repository, int id, UpdateGameDto gameDto) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame is null)
            {
                return TypedResults.NotFound();
            }

            existingGame.Name = gameDto.Name;
            existingGame.Genre = gameDto.Genre;
            existingGame.Price = gameDto.Price;
            existingGame.ReleaseDate = gameDto.ReleaseDate;
            existingGame.ImageUri = gameDto.ImageUri;

            await repository.UpdateAsync(existingGame);

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Updates a game")
        .WithDescription("Updates all game properties for the game that has the specified id");

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            var gameToDelete = await repository.GetAsync(id);

            if (gameToDelete is not null)
            {
                await repository.DeleteAsync(id);
            }

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Deletes a game")
        .WithDescription("Deletes the game that has the specified id");

        return group;
    }
}