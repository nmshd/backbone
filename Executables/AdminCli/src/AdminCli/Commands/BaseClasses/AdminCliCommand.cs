using System.CommandLine;
using System.Text.Json;

namespace Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;

public abstract class AdminCliCommand : Command
{
    protected static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS =
        new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected readonly ServiceLocator _serviceLocator;

    protected AdminCliCommand(string name, ServiceLocator serviceLocator, string? description = null) : base(name, description)
    {
        _serviceLocator = serviceLocator;
    }
}
