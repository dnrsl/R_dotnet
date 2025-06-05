using DevHabit.Api.Database;
using DevHabit.Api.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddXmlSerializerFormatters();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("Database"),
            npgsqlOptions => npgsqlOptions
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
        .UseSnakeCaseNamingConvention());

builder.Services.AddOpenTelemetry()
.ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
.WithTracing(tracing => tracing
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddNpgsql())
.WithMetrics(metrics => metrics
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddRuntimeInstrumentation())
.UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

//app.UseAuthorization(); belum digunakan

app.MapControllers();

await app.RunAsync();
