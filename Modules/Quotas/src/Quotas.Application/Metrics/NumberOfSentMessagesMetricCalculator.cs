using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain;

namespace Backbone.Quotas.Application.Metrics;
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
