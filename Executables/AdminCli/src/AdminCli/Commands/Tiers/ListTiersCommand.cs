using System.CommandLine;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using MediatR;

namespace Backbone.Modules.Devices.AdminCli.Commands.Tiers;

public class ListTiersCommand : AdminCliDbCommand
{
    public ListTiersCommand(ServiceLocator serviceLocator) : base("list", serviceLocator, "List all existing Tiers")
    {
        this.SetHandler(ListTiers, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION);
    }

    private async Task ListTiers(string dbProvider, string dbConnectionString)
    {
        var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

        var response = await mediator.Send(new ListTiersQuery(new PaginationFilter()), CancellationToken.None);

        Console.WriteLine("The following tiers are configured:");

        foreach (var client in response)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, JSON_SERIALIZER_OPTIONS));
        }
    }
}
