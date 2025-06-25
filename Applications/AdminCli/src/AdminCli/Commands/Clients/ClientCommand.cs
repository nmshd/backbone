using System.CommandLine;

namespace Backbone.AdminCli.Commands.Clients;

public class ClientCommand : Command
{
    public ClientCommand(CreateClientCommand createClientCommand, ListClientsCommand listClientsCommand, DeleteClientsCommand deleteClientsCommand) : base("client")
    {
        Subcommands.Add(createClientCommand);
        Subcommands.Add(listClientsCommand);
        Subcommands.Add(deleteClientsCommand);
    }
}
