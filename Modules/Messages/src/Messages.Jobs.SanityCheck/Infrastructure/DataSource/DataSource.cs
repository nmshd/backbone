using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Messages.Jobs.SanityCheck.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;
    private readonly IMessagesRepository _messagesRepository;

    public DataSource(IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IMessagesRepository messagesRepository)
    {
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _messagesRepository = messagesRepository;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_blobOptions.RootFolder);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MessageId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        var allMessages = await _messagesRepository.FindAll(new PaginationFilter() { PageNumber = 1, PageSize = int.MaxValue });
        return allMessages.ItemsOnPage.Select(u => u.Id);
    }
}
