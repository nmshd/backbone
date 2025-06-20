using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using MediatR;

namespace Backbone.AdminCli.Commands.Tiers;

public class CreateTierCommand : AdminCliCommand
{
    public CreateTierCommand(IMediator mediator) : base(mediator, "create", "Create a new Tier")
    {
        var name = new Option<string>("--name")
        {
            IsRequired = true,
            Description = "The name of the Tier."
        };

        AddOption(name);

        this.SetHandler(CreateTier, name);
    }

    private async Task CreateTier(string name)
    {
        var response = await _mediator.Send(new Modules.Devices.Application.Tiers.Commands.CreateTier.CreateTierCommand { Name = name }, CancellationToken.None);

        Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));
    }
}
