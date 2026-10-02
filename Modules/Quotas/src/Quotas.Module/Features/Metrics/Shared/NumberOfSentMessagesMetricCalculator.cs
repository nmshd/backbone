using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.Shared;

public class NumberOfSentMessagesMetricCalculator : IMetricCalculator
{
    private readonly IMessagesRepository _messagesRepository;

    public NumberOfSentMessagesMetricCalculator(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfMessages = await _messagesRepository.Count(identityAddress, from, to, cancellationToken);
        return numberOfMessages;
    }
}
