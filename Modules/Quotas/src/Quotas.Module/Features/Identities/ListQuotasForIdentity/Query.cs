using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.ListQuotasForIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListQuotasForIdentityQuery")]
public class Query : IRequest<Response>;
