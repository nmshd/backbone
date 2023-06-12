using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class NumberOfSentMessagesCalculator : IMetricCalculator
{
    private readonly IMessagesRepository _messagesRepository;

    public NumberOfSentMessagesCalculator(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task<uint> CalculateUsageAsync(DateTime from, DateTime to, string identityAddress)
    {
        var numberOfMessages = await _messagesRepository.Count(identityAddress, from, to);
        return numberOfMessages;
    }
}
