using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder HasVersion<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        if (builder.Metadata.Model.IsNpgsql())
        {
            return builder.Property<uint>("Version").IsRowVersion();
        }
        else if (builder.Metadata.Model.IsSqlServer())
        {
            return builder.Property<byte[]>("Version").IsRowVersion();
        }
        else
        {
            throw new NotSupportedException($"No column type for a version has been defined for the database provider '{builder.Metadata.Model.GetDbProvider()}'");
        }
    }
}
