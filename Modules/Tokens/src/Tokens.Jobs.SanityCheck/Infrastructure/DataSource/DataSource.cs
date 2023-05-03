using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;

namespace Tokens.Jobs.SanityCheck.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly TokensRepositoryOptions _repositoryOptions;
    private readonly ITokensRepository _tokensRepository;

    public DataSource(IBlobStorage blobStorage, IOptions<TokensRepositoryOptions> repositoryOptions, ITokensRepository tokensRepository)
    {
        _blobStorage = blobStorage;
        _repositoryOptions = repositoryOptions.Value;
        _tokensRepository = tokensRepository;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_repositoryOptions.BlobRootFolder);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _tokensRepository.GetAllTokenIds(includeExpired: true);
    }
}
