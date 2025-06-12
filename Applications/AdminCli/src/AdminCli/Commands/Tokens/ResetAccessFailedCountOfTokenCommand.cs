using System.CommandLine;
using Backbone.AdminCli.Commands.BaseClasses;
using MediatR;

namespace Backbone.AdminCli.Commands.Tokens;

public class ResetAccessFailedCountOfTokenCommand : AdminCliCommand
{
    public ResetAccessFailedCountOfTokenCommand(IMediator mediator) : base(mediator, "reset-access-failed-count",
        "Resets the number of failed access attempts of the Token with the given id.")
    {
        var tokenId = new Option<string>("--id")
        {
            IsRequired = true,
            Description = "The id of the Token of which the access failed count should be resetted."
        };

        AddOption(tokenId);

        this.SetHandler(ResetAccessFailedCount, tokenId);
    }

    private async Task ResetAccessFailedCount(string tokenId)
    {
        await _mediator.Send(new Modules.Tokens.Application.Tokens.Commands.ResetAccessFailedCountOfToken.ResetAccessFailedCountOfTokenCommand { TokenId = tokenId }, CancellationToken.None);

        Console.WriteLine(@"Access failed count has been reset.");
    }
}
