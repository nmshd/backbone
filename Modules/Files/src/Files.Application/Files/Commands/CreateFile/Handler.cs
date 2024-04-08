using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Application.IntegrationEvents.Out;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class Handler : IRequestHandler<CreateFileCommand, CreateFileResponse>
{
    private readonly IMapper _mapper;
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;

    public Handler(IUserContext userContext, IMapper mapper, IFilesRepository filesRepository, IEventBus eventBus)
    {
        _userContext = userContext;
        _mapper = mapper;
        _filesRepository = filesRepository;
        _eventBus = eventBus;
    }

    public async Task<CreateFileResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var file = new File(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.Owner,
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

        _eventBus.Publish(new FileUploadedIntegrationEvent(file));

        var response = _mapper.Map<CreateFileResponse>(file);

        return response;
    }
}
