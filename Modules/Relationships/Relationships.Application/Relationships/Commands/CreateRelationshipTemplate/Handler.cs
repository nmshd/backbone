using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Relationships.Domain.Entities;

namespace Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IBlobStorage _blobStorage;
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _blobStorage = blobStorage;
    }

    public async Task<CreateRelationshipTemplateResponse> Handle(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new RelationshipTemplate(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.MaxNumberOfAllocations,
            request.ExpiresAt,
            request.Content);

        if(template.Content != null)
            _blobStorage.Add(template.Id, template.Content);
        await _blobStorage.SaveAsync();

        await _dbContext.Set<RelationshipTemplate>().AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CreateRelationshipTemplateResponse>(template);
    }
}
