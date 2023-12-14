using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.Identities.Commands;
public class RequestWithIdentityAddressValidator : AbstractValidator<RequestWithIdentityAddress>
{
    public RequestWithIdentityAddressValidator()
    {
        RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
    }
}
