using Backbone.Modules.Files.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class GetFileContentQuery : IRequest<GetFileContentResponse>
{
    public FileId Id { get; set; }
}
