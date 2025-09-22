using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class Handler : IRequestHandler<CreateFileCommand, CreateFileResponse>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IFilesRepository filesRepository)
    {
        _userContext = userContext;
        _filesRepository = filesRepository;
    }

    public async Task<CreateFileResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var file = new File(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.OwnerSignature,
            request.CipherHash,
            request.FileContent,
            request.FileContent.LongLength,
            request.ExpiresAt,
            request.EncryptedProperties
        );

        await _filesRepository.Add(
            file,
            cancellationToken
        );

        return new CreateFileResponse(file);
    }
}
