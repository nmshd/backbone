using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Abstractions;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

public class Handler : IRequestHandler<Command>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
        _userContext = userContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Get(FileId.Parse(request.Id), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));

        file.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _filesRepository.Delete(file, cancellationToken);
    }
}
