using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public static class SensitiveDataEnricherExtensions
{
    private const string MASKED_DATA_PLACEHOLDER = "{maskedData}";

    public static void AddSensitiveDataMasks(this SensitiveDataEnricherOptions options)
    {
        options.MaskValue = MASKED_DATA_PLACEHOLDER;

        options.MaskingOperators.Add(IdentityAddressMaskingOperator.Create());
        options.MaskingOperators.Add(BackboneIdMaskingOperator.ForId(DeviceId.PREFIX, DeviceId.MAX_LENGTH));
        options.MaskingOperators.Add(BackboneIdMaskingOperator.ForId(Username.PREFIX, Username.MAX_LENGTH));
    }
}
