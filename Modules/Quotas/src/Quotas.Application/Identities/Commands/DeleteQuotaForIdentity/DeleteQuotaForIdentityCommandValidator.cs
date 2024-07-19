using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class DeleteQuotaForIdentityCommandValidator : AbstractValidator<DeleteQuotaForIdentityCommand>
{
    public DeleteQuotaForIdentityCommandValidator()
    {
        RuleFor(c => c.IdentityAddress).Must(IdentityAddress.IsValid);
        RuleFor(c => c.IndividualQuotaId).DetailedNotEmpty();
    }
}
