using Backbone.AdminCli.Commands.BaseClasses;

namespace Backbone.AdminCli.Commands.Clients;

public class ClientCommand : AdminCliCommand
{
    public ClientCommand(ServiceLocator serviceLocator) : base("client", serviceLocator)
    {
        AddCommand(new CreateClientCommand(serviceLocator));
        AddCommand(new ListClientsCommand(serviceLocator));
        AddCommand(new DeleteClientsCommand(serviceLocator));
    }
}
