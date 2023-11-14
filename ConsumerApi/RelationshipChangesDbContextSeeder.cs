﻿using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
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
        await FillContentColumnsFromBlobStorage(context);
    }

    private async Task FillContentColumnsFromBlobStorage(RelationshipsDbContext context)
    {
        // _blobRootFolder is null when blob storage configuration is not provided, meaning the content of database entries should not be loaded from blob storage
        if (_blobRootFolder == null || _blobStorage == null)
            return;

        await FillRelationshipChangesWithMissingContent(context);

        await FillRelationshipTemplatesWithMissingContent(context);
    }

    private async Task FillRelationshipChangesWithMissingContent(RelationshipsDbContext context)
    {
        var relationshipChangesWithMissingContent = await context.RelationshipChanges
            .Where(rc => rc.Request.Content == null || (rc.Response != null && rc.Response.Content == null)).ToListAsync();

        foreach (var relationshipChange in relationshipChangesWithMissingContent)
        {
            if (relationshipChange.Request.Content == null)
            {
                var blobRequestContent = await _blobStorage!.FindAsync(_blobRootFolder!, relationshipChange.Request.Id);
                relationshipChange.Request.LoadContent(blobRequestContent);
                context.RelationshipChanges.Update(relationshipChange);
            }

            if (relationshipChange.Response is { Content: null })
            {
                try
                {
                    var blobResponseContent = await _blobStorage!.FindAsync(_blobRootFolder!, relationshipChange.Response.Id);
                    relationshipChange.Response.LoadContent(blobResponseContent);
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

    private async Task FillRelationshipTemplatesWithMissingContent(RelationshipsDbContext context)
    {
        var relationshipTemplatesWithMissingContent = await context.RelationshipTemplates
            .Where(rt => rt.Content == null).ToListAsync();

        foreach (var relationshipTemplate in relationshipTemplatesWithMissingContent)
        {
            if (relationshipTemplate.Content == null)
            {
                var blobContent = await _blobStorage!.FindAsync(_blobRootFolder!, relationshipTemplate.Id);
                relationshipTemplate.LoadContent(blobContent);
                context.RelationshipTemplates.Update(relationshipTemplate);
            }

            await context.SaveChangesAsync();
        }
    }
}
