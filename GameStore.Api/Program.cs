using GameStore.Api.Endpoints;
using GameStore.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var gameStoreConnectionString = configuration.GetConnectionString("GameStoreContext");

builder.Services.AddSingleton<IGamesRepository, InMemGamesRepository>(); 

var app = builder.Build();

app.MapGameEndpoints();

app.Run();
