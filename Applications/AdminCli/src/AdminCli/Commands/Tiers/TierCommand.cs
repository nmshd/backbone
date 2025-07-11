using System.CommandLine;

namespace Backbone.AdminCli.Commands.Tiers;

public class TierCommand : Command
{
    public TierCommand(ListTiersCommand listTiersCommand, CreateTierCommand createTierCommand) : base("tier")
    {
        Subcommands.Add(listTiersCommand);
        Subcommands.Add(createTierCommand);
    }
}
