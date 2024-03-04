using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetDatawallet;

public class GetDatawalletQuery : IRequest<DatawalletDTO>;
