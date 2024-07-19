using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;

public class Validator : AbstractValidator<GetIdentityQuery>
{
    public Validator()
    {
        RuleFor(x => x.Address).Must(IdentityAddress.IsValid);
    }
}
