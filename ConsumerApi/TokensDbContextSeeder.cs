using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi;

public class TokensDbContextSeeder : IDbSeeder<TokensDbContext>
{
    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;

    public TokensDbContextSeeder(IServiceProvider serviceProvider)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<TokensRepositoryOptions>>()!.Value.BlobRootFolder;
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

        var tokensWithMissingContents = await context.Tokens.Where(t => t.Content == null).ToListAsync();

        foreach (var token in tokensWithMissingContents)
        {
            var blobTokenContent = await _blobStorage!.FindAsync(_blobRootFolder!, token.Id);
            token.Content = blobTokenContent;
            context.Tokens.Update(token);
        }

        await context.SaveChangesAsync();
    }
}
