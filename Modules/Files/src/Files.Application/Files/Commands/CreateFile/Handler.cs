using AutoMapper;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class Handler : IRequestHandler<CreateFileCommand, CreateFileResponse>
{
    private readonly IMapper _mapper;
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IFilesRepository filesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _filesRepository = filesRepository;
    }

    public async Task<CreateFileResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var file = new File(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.Owner,
            request.OwnerSignature ?? Array.Empty<byte>(),
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
        
        var response = _mapper.Map<CreateFileResponse>(file);

        return response;
    }
}
