using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery

namespace Backbone.ConsumerApi;

public class RelationshipChangesDbContextSeeder : IDbSeeder<RelationshipsDbContext>
{
    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;

    public RelationshipChangesDbContextSeeder(IServiceProvider serviceProvider)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()!.Value.RootFolder;
    }

    public async Task SeedAsync(RelationshipsDbContext context)
    {
        // _blobRootFolder is null when blob storage configuration is not provided, meaning the content of database entries should not be loaded from blob storage
        if (_blobRootFolder == null)
            return;

        var relationshipChanges = await context.RelationshipChanges
            .Where(rc =>  rc.Request.Content == null || (rc.Response != null && rc.Response.Content == null)).ToListAsync();

        foreach (var relationshipChange in relationshipChanges)
        {
            if (relationshipChange.Request != null && relationshipChange.Request.Content == null)
            {
                relationshipChange.Request.LoadContent(await _blobStorage!.FindAsync(_blobRootFolder, relationshipChange.Request.Id));
                context.RelationshipChanges.Update(relationshipChange);
            }

            if (relationshipChange.Response is { Content: null })
            {
                try
                {
                    relationshipChange.Response.LoadContent(await _blobStorage!.FindAsync(_blobRootFolder, relationshipChange.Response.Id));
                    context.RelationshipChanges.Update(relationshipChange);
                }
                catch (NotFoundException)
                {
                    // response content is optional so we can ignore this
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
