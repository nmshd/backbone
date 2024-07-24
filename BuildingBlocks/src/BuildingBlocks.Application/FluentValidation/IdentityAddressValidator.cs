using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public class IdentityAddressValidator : AbstractValidator<string>
{
    public IdentityAddressValidator()
    {
        RuleFor(x => x).Must(IdentityAddress.IsValid);
    }
}
