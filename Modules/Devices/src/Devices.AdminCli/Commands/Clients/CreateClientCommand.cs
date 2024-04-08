using System.CommandLine;
using System.Text.Json;
using Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;
using MediatR;

namespace Backbone.Modules.Devices.AdminCli.Commands.Clients;

public class CreateClientCommand : AdminCliDbCommand
{
    public CreateClientCommand(ServiceLocator serviceLocator) : base("create", serviceLocator, "Create an OAuth client")
    {
        var clientId = new Option<string>("--clientId")
        {
            IsRequired = false,
            Description = "The clientId of the OAuth client. Default: a randomly generated string."
        };
        var displayName = new Option<string>("--displayName")
        {
            IsRequired = false,
            Description = "The displayName of the OAuth client. Default: the clientId."
        };

        var clientSecret = new Option<string>("--clientSecret")
        {
            IsRequired = false,
            Description = "The clientSecret of the OAuth client. Default: a randomly generated string."
        };

        var defaultTierId = new Option<string>("--defaultTier")
        {
            IsRequired = true,
            Description = "The id or name of the Tier that should be assigned to all Identities created with this OAuth client."
        };

        var maxIdentities = new Option<int?>("--maxIdentities")
        {
            IsRequired = false,
            Description = "The maximum number of Identities that can be created with this OAuth client."
        };

        AddOption(clientId);
        AddOption(displayName);
        AddOption(clientSecret);
        AddOption(defaultTierId);
        AddOption(maxIdentities);

        this.SetHandler(CreateClient, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, clientId, displayName, clientSecret,
            defaultTierId, maxIdentities);
    }

    private async Task CreateClient(string dbProvider, string dbConnectionString, string? clientId,
        string? displayName, string? clientSecret, string defaultTier, int? maxIdentities)
    {
        var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

        var response = await mediator.Send(new Application.Clients.Commands.CreateClient.CreateClientCommand(clientId, displayName, clientSecret, defaultTier, maxIdentities), CancellationToken.None);

        Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }
}
