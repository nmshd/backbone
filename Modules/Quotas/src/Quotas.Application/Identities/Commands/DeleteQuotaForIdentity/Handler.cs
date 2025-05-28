using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class Handler : IRequestHandler<DeleteQuotaForIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteQuotaForIdentityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Deleting individual quota with id: '{individualQuotaId}'.", request.IndividualQuotaId);

        var identity = await _identitiesRepository.Get(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var quotaIdResult = QuotaId.Create(request.IndividualQuotaId);
        if (quotaIdResult.IsFailure)
            throw new DomainException(quotaIdResult.Error);

        var result = identity.DeleteIndividualQuota(quotaIdResult.Value);
        if (result.IsFailure)
            throw new DomainException(result.Error);

        await _identitiesRepository.Update(identity, cancellationToken);

        _logger.DeletedQuota(request.IndividualQuotaId);
    }
}

internal static partial class DeleteQuotaForIdentityLogs
{
    [LoggerMessage(
        EventId = 247156,
        EventName = "Quotas.DeleteQuotaForIdentity.DeletedQuota",
        Level = LogLevel.Information,
        Message = "Successfully deleted individual quota with id '{individualQuotaId}'.")]
    public static partial void DeletedQuota(this ILogger logger, string individualQuotaId);
}
