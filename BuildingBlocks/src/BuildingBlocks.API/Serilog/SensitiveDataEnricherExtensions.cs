using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public static class SensitiveDataEnricherExtensions
{
    private const string MASKED_DATA_PLACEHOLDER = "**MASKED**";

    public static void AddSensitiveDataMasks(this SensitiveDataEnricherOptions options)
    {
        options.MaskValue = MASKED_DATA_PLACEHOLDER;

        options.MaskingOperators.Add(new IdentityAddressMaskingOperator());
        options.MaskingOperators.Add(new StronglyTypedIdMaskingOperator(DeviceId.PREFIX, DeviceId.MAX_LENGTH));
        options.MaskingOperators.Add(new StronglyTypedIdMaskingOperator(Username.PREFIX, Username.MAX_LENGTH));
    }
}
