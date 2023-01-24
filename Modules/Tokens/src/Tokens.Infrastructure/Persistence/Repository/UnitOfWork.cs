using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Tokens.Application.Infrastructure;
using Tokens.Infrastructure.Persistence.Database;

namespace Tokens.Infrastructure.Persistence.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly IBlobStorage _blobStorage;
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context, IBlobStorage blobStorage)
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
