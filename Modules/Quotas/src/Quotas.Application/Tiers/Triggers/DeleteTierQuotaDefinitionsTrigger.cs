using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using EntityFrameworkCore.Triggered;

namespace Backbone.Modules.Quotas.Application.Tiers.Triggers;

public class DeleteTierQuotaDefinitionsTrigger(ITiersRepository tiersRepository) : IAfterSaveTrigger<TierQuotaDefinition>
{
    public async Task AfterSave(ITriggerContext<TierQuotaDefinition> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Modified)
            await tiersRepository.RemoveTierQuotaDefinitionIfOrphaned(context.Entity.Id);
    }
}
