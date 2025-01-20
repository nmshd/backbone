using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public class RelationshipTemplateBag(CreateRelationshipTemplateResponse template, bool used)
{
    public CreateRelationshipTemplateResponse Template { get; } = template;
    public bool Used { get; set; } = used;
}
