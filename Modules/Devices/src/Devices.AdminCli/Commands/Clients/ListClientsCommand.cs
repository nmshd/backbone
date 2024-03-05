using System.CommandLine;
using System.Text.Json;
using Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using MediatR;

namespace Backbone.Modules.Devices.AdminCli.Commands.Clients;

public class ListClientsCommand : AdminCliDbCommand
{
    public ListClientsCommand(ServiceLocator serviceLocator) : base("list", serviceLocator, "List all existing OAuth clients")
    {
        this.SetHandler(ListClients, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION);
    }

    private async Task ListClients(string dbProvider, string dbConnectionString)
    {
        var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

        var response = await mediator.Send(new ListClientsQuery(), CancellationToken.None);

        Console.WriteLine("The following clients are configured:");

        foreach (var client in response)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, JSON_SERIALIZER_OPTIONS));
        }
    }
}
