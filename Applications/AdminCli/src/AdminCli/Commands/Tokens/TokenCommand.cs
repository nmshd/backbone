using System.CommandLine;

namespace Backbone.AdminCli.Commands.Tokens;

public class TokenCommand : Command
{
    public TokenCommand(ResetAccessFailedCountOfTokenCommand resetAccessFailedCountCommand) : base("token")
    {
        AddCommand(resetAccessFailedCountCommand);
    }
}
