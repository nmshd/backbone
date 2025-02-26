using System.CommandLine;

namespace Backbone.AdminCli.Commands.Tiers;

public class TierCommand : Command
{
    public TierCommand(ListTiersCommand listTiersCommand) : base("tier")
    {
        AddCommand(listTiersCommand);
    }
}
