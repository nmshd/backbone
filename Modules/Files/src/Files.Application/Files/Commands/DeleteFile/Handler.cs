using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.DeleteFile;

public class Handler : IRequestHandler<DeleteFileCommand>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
        _userContext = userContext;
    }

    public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.Id), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));

        file.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _filesRepository.Delete(file, cancellationToken);
    }
}
