using Backbone.Files.Domain.Entities;
using MediatR;

namespace Backbone.Files.Application.Files.Queries.GetFileContent;

public class GetFileContentQuery : IRequest<GetFileContentResponse>
{
    public FileId Id { get; set; }
}
