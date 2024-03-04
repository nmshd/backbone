using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery

namespace Backbone.ConsumerApi;

public class TokensDbContextSeeder : IDbSeeder<TokensDbContext>
{
    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;

    public TokensDbContextSeeder(IServiceProvider serviceProvider)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()?.Value.RootFolder;
    }

    public async Task SeedAsync(TokensDbContext context)
    {
        await FillContentColumnsFromBlobStorage(context);
    }

    private async Task FillContentColumnsFromBlobStorage(TokensDbContext context)
    {
        // _blobRootFolder is null when blob storage configuration is not provided, meaning the content of database entries should not be loaded from blob storage
        if (_blobRootFolder == null)
            return;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var tokensWithMissingContent = await context.Tokens.Where(t => t.Content == null).ToListAsync();

        foreach (var token in tokensWithMissingContent)
        {
            try
            {
                var blobTokenContent = await _blobStorage!.FindAsync(_blobRootFolder!, token.Id);
                token.LoadContent(blobTokenContent);
                context.Tokens.Update(token);
            }
            catch (NotFoundException)
            {
                // due to missing validation, it was possible to create a relationship template without request content;
                // therefore we cannot tell whether this exception is an error or not
            }
        }

        await context.SaveChangesAsync();
    }
}
