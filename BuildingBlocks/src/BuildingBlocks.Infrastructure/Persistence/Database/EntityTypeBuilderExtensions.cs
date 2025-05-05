using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<object> HasVersion<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object>> propertyExpression) where TEntity : class
    {
        if (builder.Metadata.Model.IsNpgsql())
        {
            return builder.Property(propertyExpression).HasColumnType("xid").HasConversion(
                v => (uint)v,
                v => v
            );
        }
        else
        {
            return builder.Property(propertyExpression).HasColumnType("rowversion").HasConversion(
                v => (byte[])v,
                v => v
            );
        }
    }
}
