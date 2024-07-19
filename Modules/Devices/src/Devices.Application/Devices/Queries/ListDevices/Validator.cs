using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;

public class Validator : AbstractValidator<ListDevicesQuery>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).Must(DeviceId.IsValid);
    }
}
