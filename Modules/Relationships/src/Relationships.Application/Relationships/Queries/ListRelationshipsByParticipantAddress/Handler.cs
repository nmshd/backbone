﻿using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
public class Handler : IRequestHandler<ListRelationshipsByParticipantAddressQuery, ListRelationshipsByParticipantAddressResponse>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IMapper mapper, IRelationshipsRepository relationshipsRepository)
    {
        _mapper = mapper;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<ListRelationshipsByParticipantAddressResponse> Handle(ListRelationshipsByParticipantAddressQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipsRepository.FindRelationshipsWithParticipant(request.ParticipantAddress, request.PaginationFilter, cancellationToken, track: false);

        return new ListRelationshipsByParticipantAddressResponse(_mapper.Map<RelationshipDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
