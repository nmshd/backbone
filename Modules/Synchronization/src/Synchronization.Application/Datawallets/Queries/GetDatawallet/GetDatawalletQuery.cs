using Backbone.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Synchronization.Application.Datawallets.Queries.GetDatawallet;

public class GetDatawalletQuery : IRequest<DatawalletDTO> { }
