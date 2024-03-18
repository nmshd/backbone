using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretCommandValidator : AbstractValidator<ChangeClientSecretCommand>
{
    public ChangeClientSecretCommandValidator()
    {
        RuleFor(c => c.ClientId).DetailedNotEmpty();
    }
}
