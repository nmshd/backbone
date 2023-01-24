using MediatR;
using Synchronization.Application.Datawallets.DTOs;

namespace Synchronization.Application.Datawallets.Queries.GetDatawallet;

public class GetDatawalletQuery : IRequest<DatawalletDTO> { }
