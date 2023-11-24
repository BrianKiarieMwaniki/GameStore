using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddRepositories(configuration); 

var app = builder.Build();

app.Services.InitializeDb();

app.MapGameEndpoints();

app.Run();
