using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class ValidateFileOwnershipTokenQuery : IRequest<ValidateFileOwnershipTokenResponse>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}
