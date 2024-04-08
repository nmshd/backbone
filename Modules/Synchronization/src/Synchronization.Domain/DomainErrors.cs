using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Synchronization.Domain;
public static class DomainErrors
{
    public static class Datawallet
    {
        public static DomainError CannotDowngrade(ushort currentVersion, ushort targetVersion)
        {
            return new("error.platform.datawallet.cannotDowngrade",
                $"You cannot upgrade from version '{currentVersion}' to '{targetVersion}', because it is not possible to upgrade to lower versions.");
        }

        public static DomainError DatawalletVersionOfModificationTooHigh(ushort datawalletVersion, ushort modificationVersion)
        {
            return new("error.platform.datawallet.datawalletVersionOfModificationTooHigh",
                $"Cannot add modifications with DatawalletVersion '{modificationVersion}', because the datawallet only has version '{datawalletVersion}'.");
        }
    }
}
