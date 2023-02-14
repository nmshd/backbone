using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IRelationshipsDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IContentStore _contentStore;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _contentStore = contentStore;
    }

    public async Task<CreateRelationshipTemplateResponse> Handle(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new RelationshipTemplate(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.MaxNumberOfAllocations,
            request.ExpiresAt,
            request.Content);

        await _contentStore.SaveContentOfTemplate(template);

        await _dbContext.Set<RelationshipTemplate>().AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CreateRelationshipTemplateResponse>(template);
    }
}
