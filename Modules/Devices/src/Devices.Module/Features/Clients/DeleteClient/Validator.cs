using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Clients.DeleteClient;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.ClientId).DetailedNotEmpty();
    }
}
