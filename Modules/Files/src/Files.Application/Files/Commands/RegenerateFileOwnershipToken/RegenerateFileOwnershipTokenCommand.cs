using Backbone.Modules.Files.Application.Files.DTOs;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenCommand : IRequest<FileOwnershipTokenDTO>
{
    public required string Id { get; set; }
}
