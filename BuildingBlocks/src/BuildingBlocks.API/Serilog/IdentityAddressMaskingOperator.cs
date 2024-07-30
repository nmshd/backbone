using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class IdentityAddressMaskingOperator : RegexMaskingOperator
{
    public IdentityAddressMaskingOperator() : base(@"did:e:[a-zA-Z0-9]+:dids:[a-zA-Z0-9]{22}") { }
}
