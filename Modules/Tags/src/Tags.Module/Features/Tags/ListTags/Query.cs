using MediatR;

namespace Backbone.Modules.Tags.Module.Features.Tags.ListTags;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTagsQuery")]
public class Query : IRequest<Response>;
