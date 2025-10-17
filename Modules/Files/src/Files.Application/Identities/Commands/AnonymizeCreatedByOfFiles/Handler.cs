using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;
using static Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Identities.Commands.AnonymizeCreatedByOfFiles;

public class Handler : IRequestHandler<AnonymizeCreatedByOfFilesCommand>
{
    private readonly IFilesRepository _filesRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(IFilesRepository filesRepository, IOptions<ApplicationConfiguration> applicationOptions)
    {
        _filesRepository = filesRepository;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeCreatedByOfFilesCommand request, CancellationToken cancellationToken)
    {
        var addressOfIdentityToAnonymize = IdentityAddress.ParseUnsafe(request.IdentityAddress);
        var anonymizedAddress = IdentityAddress.GetAnonymized(_applicationConfiguration.DidDomainName);

        await _filesRepository.AnonymizeCreatedByOfFiles(WasCreatedBy(addressOfIdentityToAnonymize), anonymizedAddress, cancellationToken);
    }
}
