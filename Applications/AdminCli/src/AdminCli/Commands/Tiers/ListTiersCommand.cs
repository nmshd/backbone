using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using MediatR;

namespace Backbone.AdminCli.Commands.Tiers;

public class ListTiersCommand : AdminCliCommand
{
    public ListTiersCommand(IMediator mediator) : base(mediator, "list", "List all existing Tiers")
    {
        SetAction((ParseResult parseResult, CancellationToken token) => ListTiers());
    }

    private async Task ListTiers()
    {
        var response = await _mediator.Send(new ListTiersQuery { PaginationFilter = new PaginationFilter() }, CancellationToken.None);

        Console.WriteLine(@"The following tiers are configured:");

        foreach (var client in response)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, JSON_SERIALIZER_OPTIONS));
        }
    }
}
