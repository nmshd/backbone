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
        if (_blobRootFolder == null)
            return;

        var tokens = await context.Tokens.Where(t => t.Content == null).ToListAsync();

        foreach (var token in tokens)
        {
            token.Content = await _blobStorage!.FindAsync(_blobRootFolder!, token.Id);
            context.Tokens.Update(token);
        }

        await context.SaveChangesAsync();
    }
}
