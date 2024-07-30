using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public static class SensitiveDataEnricherExtensions
{
    private const string MASKED_DATA_PLACEHOLDER = "{maskedData}";

    public static void AddSensitiveDataMasks(this SensitiveDataEnricherOptions options)
    {
        options.MaskValue = MASKED_DATA_PLACEHOLDER;

        options.MaskingOperators.Add(new IdentityAddressMaskingOperator());
        options.MaskingOperators.Add(new BackboneIdMaskingOperator("DVC", 17));
        options.MaskingOperators.Add(new BackboneIdMaskingOperator("USR"));
    }
}
