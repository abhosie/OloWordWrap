using OloWordWrap.Api;
using OloWordWrap.Application;
using OloWordWrap.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApi(builder.Configuration)
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Before the controllers, so a client over its limit is turned away before any work is done.
app.UseRateLimiter();

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
