using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;

public class Validator : AbstractValidator<ChangeClientSecretCommand>
{
    public Validator()
    {
        RuleFor(c => c.ClientId).DetailedNotEmpty();
    }
}
