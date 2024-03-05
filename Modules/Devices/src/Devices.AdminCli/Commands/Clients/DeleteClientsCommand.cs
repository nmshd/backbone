using System.CommandLine;
using Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;
using MediatR;

namespace Backbone.Modules.Devices.AdminCli.Commands.Clients;

public class DeleteClientsCommand : AdminCliDbCommand
{
    public DeleteClientsCommand(ServiceLocator serviceLocator) : base("delete", serviceLocator, "Deletes the OAuth clients with the given clientId's")
    {
        var clientIds = new Argument<string[]>("clientIds")
        {
            Arity = ArgumentArity.OneOrMore,
            Description = "The clientId's that should be deleted."
        };

        AddArgument(clientIds);

        this.SetHandler(DeleteClients, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, clientIds);
    }

    private async Task DeleteClients(string dbProvider, string dbConnectionString, string[] clientIds)
    {
        var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

        foreach (var clientId in clientIds)
        {
            try
            {
                await mediator.Send(new DeleteClientCommand(clientId), CancellationToken.None);
                Console.WriteLine($"Successfully deleted client '{clientId}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
