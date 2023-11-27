using GameStore.Api.Authorization;
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddRepositories(configuration);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddGameStoreAuthorization();

var app = builder.Build();

await app.Services.InitializeDbAsync();

app.UseHttpLogging();

app.MapGameEndpoints();

app.Run();
