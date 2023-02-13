using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Tokens.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ITokenRepository _tokenRepository;

        public DataSource(IBlobStorage blobStorage, ITokenRepository tokenRepository)
        {
            _blobStorage = blobStorage;
            _tokenRepository = tokenRepository;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobStorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _tokenRepository.GetAllTokenIds();
        }
    }
}
