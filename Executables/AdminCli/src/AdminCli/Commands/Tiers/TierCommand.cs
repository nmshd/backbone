using Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;

namespace Backbone.Modules.Devices.AdminCli.Commands.Tiers;

public class TierCommand : AdminCliCommand
{
    public TierCommand(ServiceLocator serviceLocator) : base("tier", serviceLocator)
    {
        AddCommand(new ListTiersCommand(serviceLocator));
    }
}
