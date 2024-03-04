using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class ModelBuilderExtensions
{
    public static ModelBuilder UseValueConverter(this ModelBuilder modelBuilder, ValueConverter converter)
    {
        var type = converter.ModelClrType;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType
                .ClrType
                .GetProperties()
                .Where(p => p.PropertyType == type);

            foreach (var property in properties)
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(converter);
        }

        return modelBuilder;
    }
}
