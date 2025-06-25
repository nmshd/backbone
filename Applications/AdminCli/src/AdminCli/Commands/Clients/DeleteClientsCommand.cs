using System.CommandLine;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;
using MediatR;

namespace Backbone.AdminCli.Commands.Clients;

public class DeleteClientsCommand : AdminCliCommand
{
    public DeleteClientsCommand(IMediator mediator) : base(mediator, "delete", "Deletes the OAuth clients with the given clientId's")
    {
        var clientIds = new Argument<string[]>("clientIds")
        {
            Arity = ArgumentArity.OneOrMore,
            Description = "The clientId's that should be deleted."
        };

        Arguments.Add(clientIds);

        SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            var clientIdsValue = parseResult.GetRequiredValue(clientIds);
            return DeleteClients(clientIdsValue);
        });
    }

    private async Task DeleteClients(string[] clientIds)
    {
        foreach (var clientId in clientIds)
        {
            try
            {
                await _mediator.Send(new DeleteClientCommand { ClientId = clientId }, CancellationToken.None);
                Console.WriteLine($@"Successfully deleted client '{clientId}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
