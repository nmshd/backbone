using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class UpdateIdentityCommandValidator : AbstractValidator<UpdateIdentityCommand>
{
    public UpdateIdentityCommandValidator()
    {
        RuleFor(c => c.Address).DetailedNotEmpty();
        RuleFor(c => c.TierId).DetailedNotEmpty();
    }
}
