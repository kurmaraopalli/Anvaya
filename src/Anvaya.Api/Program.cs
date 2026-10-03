using Anvaya.Core.Interfaces;
using Anvaya.Core.Services;
using Anvaya.Infrastructure.Repositories;
using Anvaya.Infrastructure.Rules;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🌍 Anvaya Global Loyalty Engine API",
        Version = "v1",
        Description = "Headless, event-driven loyalty, rewards, and membership platform designed for cross-border multi-currency execution across any industry vertical."
    });
});

// Core & Infrastructure Services
builder.Services.AddSingleton<IMemberRepository, InMemoryMemberRepository>();
builder.Services.AddSingleton<ILedgerRepository, InMemoryLedgerRepository>();
builder.Services.AddSingleton<IRuleEngine, StandardRuleEngine>();
builder.Services.AddScoped<LoyaltyEngineService>();
builder.Services.AddScoped<RedemptionService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anvaya Loyalty API v1");
    c.RoutePrefix = string.Empty; // Serves interactive Swagger UI directly at root (http://localhost:5000/)
    c.DocumentTitle = "Anvaya Loyalty Engine - API Documentation";
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
