using Asp.Versioning;
using Azure.Storage.Blobs;
using GameStore.Api.Authorization;
using GameStore.Api.Cors;
using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.ErrorHandling;
using GameStore.Api.ImageUpload;
using GameStore.Api.Middleware;
using GameStore.Api.OpenAPI;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddRepositories(configuration);

builder.Services.AddAuthentication().AddJwtBearer()
                                    .AddJwtBearer("Auth0");

builder.Services.AddGameStoreAuthorization();

builder.Services.AddApiVersioning(options => {
    options.DefaultApiVersion = new(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
})
.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

builder.Services.AddGameStoreCors(configuration);

builder.Services.AddSwaggerGen()
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddEndpointsApiExplorer();

builder.Services.AddSingleton<IImageUploader>(
    new ImageUploader(
        new BlobContainerClient(
            builder.Configuration.GetConnectionString("AzureStorage"), 
            "images"
        )
    )
);

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler());

app.UseMiddleware<RequestTimingMiddleware>();

await app.Services.InitializeDbAsync();

app.UseHttpLogging();

app.MapGameEndpoints();

app.UseCors();

app.UseGameStoreSwagger();

app.Run();
