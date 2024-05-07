using Backbone.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class EntityEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Ignore(e => e.DomainEvents);
    }
}
