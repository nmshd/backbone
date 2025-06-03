using System.CommandLine;

namespace Backbone.AdminCli.Commands.Tiers;

public class TierCommand : Command
{
    public TierCommand(ListTiersCommand listTiersCommand, CreateTierCommand createTierCommand) : base("tier")
    {
        AddCommand(listTiersCommand);
        AddCommand(createTierCommand);
    }
}
