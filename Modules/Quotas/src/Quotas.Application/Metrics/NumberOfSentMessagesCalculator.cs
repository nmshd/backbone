using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class NumberOfSentMessagesCalculator : IMetricCalculator
{
    private readonly IMessagesRepository _messagesRepository;

    public NumberOfSentMessagesCalculator(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public void CalculateUsage(DateTime from, DateTime to, IdentityAddress identityAddress)
    {
        var numberOfMessages = _messagesRepository.Count(identityAddress, from, to);
    }
}
