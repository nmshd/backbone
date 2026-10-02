using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).ValidId<Query, DeviceId>();
    }
}
