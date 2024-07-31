using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class IdentityAddressMaskingOperator : RegexMaskingOperator
{
    public IdentityAddressMaskingOperator(string regexString) : base(regexString) { }
    public static IdentityAddressMaskingOperator Create() => new(IdentityAddress.IdentityAddressValidatorRegex().ToString());
}
