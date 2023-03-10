using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Infrastructure;

public interface IContentStore
{
    Task SaveContentOfTemplate(RelationshipTemplate template);
    Task FillContentOfTemplate(RelationshipTemplate template);

    Task SaveContentOfChangeRequest(RelationshipChangeRequest change);
    Task SaveContentOfChangeResponse(RelationshipChangeResponse change);

    Task FillContentOfChanges(IEnumerable<RelationshipChange> changes);
    Task FillContentOfChange(RelationshipChange change);
    Task FillContentOfTemplates(IEnumerable<RelationshipTemplate> templates);
}
