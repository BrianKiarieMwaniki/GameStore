using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
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
                            .WithParameterValidation();
        
        group.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory,
                                [AsParameters]GetGamesDtoV1 request, HttpContext context) =>
        {
            var games = await repository.GetAllAsync(request.PageNumber, request.PageSize);

            var totalCount = await repository.CountAsync();

            context.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok(games.Select(g => g.AsDtoV1()));

        })
        .MapToApiVersion(1.0);

        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game == null) return Results.NotFound();
            return Results.Ok(game.AsDtoV1());

        })
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);

        #region V2 endpoints
        group.MapGet("/", async (IGamesRepository repository, [AsParameters]GetGamesDtoV2 request, HttpContext context) =>
        {
            var games = await repository.GetAllAsync(request.PageNumber, request.PageSize);

            var totalCount = await repository.CountAsync();

            context.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok(games.Select(g => g.AsDtoV2()));

        })
        .MapToApiVersion(2.0);

        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game == null) return Results.NotFound();
            return Results.Ok(game.AsDtoV2());

        })
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0);

        #endregion

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

            return Results.CreatedAtRoute(GetGameV1EndpointName, new { id = game.Id }, game);
        })
        .MapToApiVersion(1.0)
        .RequireAuthorization(Policies.WriteAccess);

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
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            var gameToDelete = await repository.GetAsync(id);

            if (gameToDelete is null) return Results.NotFound();

            await repository.DeleteAsync(id);

            return Results.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        return group;
    }
}