using Carter;
using FluentValidation;
using Google.Authentication.Extensions;
using Streaming.SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(configurator =>
{
    configurator.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddGoogleAuthentication();
builder.Services.AddSharedRateLimiter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UsePathBase("/google");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("v1/swagger.json", "Name");
    });
    
}
else
{
    app.UsePathBase(new PathString("/" + "google"));
}


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

await app.RunAsync();