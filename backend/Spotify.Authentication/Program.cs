using Carter;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Spotify.Authentication;
using Spotify.Authentication.Abstractions.Extensions;
using Streaming.SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCarter();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMediatR(configurator =>
{
    configurator.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddSpotifyAuthentication();
builder.Services.AddSharedRateLimiter();
builder.Services.AddSwaggerGen();
builder.Services.AddWebsiteCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Spotify.Authentication v1");
    });
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

await app.RunAsync();