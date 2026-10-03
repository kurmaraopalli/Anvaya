using Anvaya.Core.Interfaces;
using Anvaya.Core.Services;
using Anvaya.Infrastructure.Repositories;
using Anvaya.Infrastructure.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Core & Infrastructure Services (Phase 1 Singleton/Scoped)
builder.Services.AddSingleton<IMemberRepository, InMemoryMemberRepository>();
builder.Services.AddSingleton<ILedgerRepository, InMemoryLedgerRepository>();
builder.Services.AddSingleton<IRuleEngine, StandardRuleEngine>();
builder.Services.AddScoped<LoyaltyEngineService>();
builder.Services.AddScoped<RedemptionService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anvaya Loyalty API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
