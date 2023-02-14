using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly IBlobStorage _blobStorage;
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context, IBlobStorage blobStorage, IOptions<TokenRepositoryOptions> repositoryOptions)
    {
        _context = context;
        _blobStorage = blobStorage;

        Tokens = new TokenRepository(context, blobStorage, repositoryOptions);
    }

    public ITokenRepository Tokens { get; }

    public async Task SaveAsync()
    {
        await _blobStorage.SaveAsync();
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
