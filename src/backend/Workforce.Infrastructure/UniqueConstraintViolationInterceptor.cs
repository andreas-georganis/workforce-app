using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.RegularExpressions;
namespace Workforce.Infrastructure;

public sealed class UniqueConstraintViolationInterceptor : SaveChangesInterceptor
{
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Exception is DbUpdateException ex &&
            ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            var propertyName = MapConstraintToProperty(ex);
            var entityName = ex.Entries.FirstOrDefault()?.Metadata.ClrType.Name;

            throw new UniqueConstraintViolationException(
                propertyName ?? "Unknown",
                $"{entityName ?? "resource"} with the same {propertyName ?? "value"} already exists.");
        }

        return Task.CompletedTask;
    }

    private static string? MapConstraintToProperty(DbUpdateException exception)
    {
        var entry = exception.Entries.FirstOrDefault();
        if (entry is null) return null;

        var sqlEx = exception.InnerException as SqlException;
        if (sqlEx is null) return null;

        // Constraint name example: "IX_Users_Email"
        var constraintName = sqlEx.Message.Split("'").ElementAtOrDefault(3);
        var parts = constraintName?.Split('_');
        if (parts is not null &&  parts.Length >= 3) return parts[^1]; // last segment
        return null;
    }
}