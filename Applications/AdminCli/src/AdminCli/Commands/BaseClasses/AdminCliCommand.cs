using System.CommandLine;
using System.Text.Json;
using MediatR;

namespace Backbone.AdminCli.Commands.BaseClasses;

public abstract class AdminCliCommand : Command
{
    protected readonly IMediator _mediator;

    protected static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS =
        new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected AdminCliCommand(IMediator mediator, string name, string? description = null) : base(name, description)
    {
        _mediator = mediator;
    }
}
