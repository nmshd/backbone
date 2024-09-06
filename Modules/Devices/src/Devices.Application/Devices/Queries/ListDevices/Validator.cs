using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class Validator : AbstractValidator<ListDevicesQuery>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).ValidId<ListDevicesQuery, DeviceId>();
    }
}
