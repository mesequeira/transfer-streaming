using Carter;
using FluentValidation;
using Google.Authentication.Authentication.Abstractions.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(configurator =>
{
    configurator.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddGoogleAuthentication();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Google.Authentication v1");
    });
}

app.MapCarter();
app.UseHttpsRedirection();
await app.RunAsync();