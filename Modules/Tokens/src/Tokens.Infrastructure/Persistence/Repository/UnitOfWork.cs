using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly IBlobStorage _blobStorage;
    private readonly TokensDbContext _context;

    public UnitOfWork(TokensDbContext context, IBlobStorage blobStorage)
    {
        _context = context;
        _blobStorage = blobStorage;

        Tokens = new TokenRepository(context, blobStorage);
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
