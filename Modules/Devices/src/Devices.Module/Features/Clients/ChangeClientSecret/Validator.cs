using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Clients.ChangeClientSecret;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.ClientId).DetailedNotEmpty();
    }
}
