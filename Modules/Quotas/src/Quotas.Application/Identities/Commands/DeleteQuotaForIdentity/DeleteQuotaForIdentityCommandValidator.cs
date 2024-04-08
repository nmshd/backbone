using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class DeleteQuotaForIdentityCommandValidator : AbstractValidator<DeleteQuotaForIdentityCommand>
{
    public DeleteQuotaForIdentityCommandValidator()
    {
        RuleFor(c => c.IdentityAddress).DetailedNotEmpty();
        RuleFor(c => c.IndividualQuotaId).DetailedNotEmpty();
    }
}
