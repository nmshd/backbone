using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;

public class MessagesRepository : IMessagesRepository
{
    private readonly MessagesDbContext _dbContext;
    private readonly DbSet<Message> _messages;
    private readonly IQueryable<Message> _readOnlyMessages;

    public MessagesRepository(MessagesDbContext dbContext)
    {
        _messages = dbContext.Messages;
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task<Message> GetWithContent(MessageId id, IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        var message = await (track ? _messages : _readOnlyMessages)
            .IncludeAll(_dbContext)
            .AsSplitQuery() // Use split query to avoid cartesian explosion. see: https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries#split-queries
            .Where(Message.HasParticipant(address))
            .FirstWithId(id, cancellationToken);

        return message;
    }

    public async Task Add(Message message, CancellationToken cancellationToken)
    {
        await _messages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountUnreceivedMessagesFromSenderToRecipient(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipient)
            .CountAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<Message>> ListMessagesWithContent(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _messages : _readOnlyMessages)
            .AsQueryable()
            .IncludeAll(_dbContext)
            .AsSplitQuery();

        if (ids.Any())
            query = query.WithIdsIn(ids);

        var messages = await query.Where(Message.HasParticipant(requiredParticipant))
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return messages;
    }

    public async Task Update(IEnumerable<Message> messages)
    {
        _dbContext.UpdateRange(messages);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Message message)
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> ListWithoutContent(Expression<Func<Message, bool>> expression, CancellationToken cancellationToken)
    {
        return await _messages
            .Include(m => m.Recipients)
            .Include(m => m.Attachments)
            .AsSplitQuery()
            .Where(expression)
            .ToListAsync(cancellationToken);
    }

    public async Task Delete(MessageId messageId, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // Type or member is obsolete; While it's true that there is an ExecuteDeleteAsync method in EF Core, it cannot be used here because it cannot be used in scenarios where table splitting is used. See https://github.com/dotnet/efcore/issues/28521 for the feature request that would allow this.
        await _messages.Where(m => m.Id == messageId).BatchDeleteAsync(cancellationToken);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
