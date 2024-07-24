using Backbone.AdminCli.Commands.BaseClasses;

namespace Backbone.AdminCli.Commands.Tiers;

public class TierCommand : AdminCliCommand
{
    public TierCommand(ServiceLocator serviceLocator) : base("tier", serviceLocator)
    {
        AddCommand(new ListTiersCommand(serviceLocator));
    }
}
