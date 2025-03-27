using BankingSystem.API.Extensions;
using BankingSystem.Application.Interfaces.Services;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Repositories;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BankingSystem.API;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var sqlConnectionString = GetSqlConnectionString(builder.Configuration);

        builder.Services.AddDbContext<BankingDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IAccountHistoryRepository, AccountHistoryRepository>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.MapType<NoContentResult>(() => new OpenApiSchema { Type = "null" });

            c.EnableAnnotations();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Aplicar Migrations automaticamente
        app.ApplyMigrations();

        app.Run();
    }

    private static string GetSqlConnectionString(IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        var server = Environment.GetEnvironmentVariable("SQL_HOST") ?? string.Empty;
        var port = Environment.GetEnvironmentVariable("SQL_PORT") ?? string.Empty;
        var user = Environment.GetEnvironmentVariable("SQL_USER") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;

        sqlConnectionString = sqlConnectionString
            .Replace("{HOST}", server)
            .Replace("{PORT}", port)
            .Replace("{USER}", user)
            .Replace("{PASSWORD}", password);

        return sqlConnectionString;
    }
}
