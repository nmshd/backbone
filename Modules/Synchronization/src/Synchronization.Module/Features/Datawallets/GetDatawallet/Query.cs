using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.GetDatawallet;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetDatawalletQuery")]
public class Query : IRequest<DatawalletDTO>;
