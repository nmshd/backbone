using Files.Domain.Entities;
using MediatR;

namespace Files.Application.Files.Queries.GetFileContent;

public class GetFileContentQuery : IRequest<GetFileContentResponse>
{
    public FileId Id { get; set; }
}
