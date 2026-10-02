using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using MediatR;
using ListClients = Backbone.Modules.Devices.Module.Features.Clients.ListClients;

namespace Backbone.AdminCli.Commands.Clients;

public class ListClientsCommand : AdminCliCommand
{
    public ListClientsCommand(IMediator mediator) : base(mediator, "list", "List all existing OAuth clients")
    {
        SetAction((ParseResult parseResult, CancellationToken token) => ListClients());
    }

    private async Task ListClients()
    {
        var response = await _mediator.Send(new ListClients.Query(), CancellationToken.None);

        Console.WriteLine(@"The following clients are configured:");

        foreach (var client in response)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, JSON_SERIALIZER_OPTIONS));
        }
    }
}
