using BankingSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace BankingSystem.API.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BankingDbContext>();

            // Verifica se existem migrations pendentes
            var pendingMigrations = dbContext.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                try
                {
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Erro ao aplicar migrations pendentes.", ex);
                }
            }
        }
    }
}