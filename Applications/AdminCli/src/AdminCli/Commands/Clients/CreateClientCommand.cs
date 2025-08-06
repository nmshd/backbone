using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using MediatR;

namespace Backbone.AdminCli.Commands.Clients;

public class CreateClientCommand : AdminCliCommand
{
    public CreateClientCommand(IMediator mediator) : base(mediator, "create", "Create an OAuth client")
    {
        var clientId = new Option<string>("--clientId")
        {
            Required = false,
            Description = "The clientId of the OAuth client. Default: a randomly generated string."
        };
        var displayName = new Option<string>("--displayName")
        {
            Required = false,
            Description = "The displayName of the OAuth client. Default: the clientId."
        };

        var clientSecret = new Option<string>("--clientSecret")
        {
            Required = false,
            Description = "The clientSecret of the OAuth client. Default: a randomly generated string."
        };

        var defaultTierId = new Option<string>("--defaultTier")
        {
            Required = true,
            Description = "The id or name of the Tier that should be assigned to all Identities created with this OAuth client."
        };

        var maxIdentities = new Option<int?>("--maxIdentities")
        {
            Required = false,
            Description = "The maximum number of Identities that can be created with this OAuth client."
        };

        Options.Add(clientId);
        Options.Add(displayName);
        Options.Add(clientSecret);
        Options.Add(defaultTierId);
        Options.Add(maxIdentities);

        SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            var clientIdValue = parseResult.GetValue(clientId);
            var displayNameValue = parseResult.GetValue(displayName);
            var clientSecretValue = parseResult.GetValue(clientSecret);
            var defaultTierIdValue = parseResult.GetRequiredValue(defaultTierId);
            var maxIdentitiesValue = parseResult.GetValue(maxIdentities);
            return CreateClient(clientIdValue, displayNameValue, clientSecretValue, defaultTierIdValue, maxIdentitiesValue);
        });
    }

    private async Task CreateClient(string? clientId, string? displayName, string? clientSecret, string defaultTier, int? maxIdentities)
    {
        var command = new Modules.Devices.Application.Clients.Commands.CreateClient.CreateClientCommand
        {
            ClientId = clientId,
            DisplayName = displayName,
            ClientSecret = clientSecret,
            DefaultTier = defaultTier,
            MaxIdentities = maxIdentities
        };

        var response = await _mediator.Send(command, CancellationToken.None);

        Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));
        Console.WriteLine(@"Please note the secret since you cannot obtain it later.");
    }
}
