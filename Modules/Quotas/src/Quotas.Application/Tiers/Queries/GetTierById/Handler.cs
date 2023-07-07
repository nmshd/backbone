using AutoMapper;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class Handler : IRequestHandler<GetTierByIdQuery, GetTierByIdResponse>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMapper _mapper;

    public Handler(ITiersRepository tiersRepository, IMapper mapper)
    {
        _tiersRepository = tiersRepository;
        _mapper = mapper;
    }

    public async Task<GetTierByIdResponse> Handle(GetTierByIdQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.Id, cancellationToken);

        var response = _mapper.Map<TierDTO>(tier);

        return new GetTierByIdResponse(response.Id, response.Name, response.Quotas);
    }
}