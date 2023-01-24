using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Relationships.Application.Infrastructure;
using Relationships.Domain.Entities;

namespace Relationships.Infrastructure.Persistence.ContentStore;

public class BlobStorageContentStore : IContentStore
{
    private readonly IBlobStorage _blobStorage;

    public BlobStorageContentStore(IBlobStorage blobStorage)
    {
        _blobStorage = blobStorage;
    }

    public async Task SaveContentOfTemplate(RelationshipTemplate template)
    {
        if (template.Content == null)
            return;

        _blobStorage.Add(template.Id, template.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task FillContentOfTemplate(RelationshipTemplate template)
    {
        var content = await _blobStorage.FindAsync(template.Id);
        template.LoadContent(content);
    }

    public async Task SaveContentOfChangeRequest(RelationshipChangeRequest changeRequest)
    {
        if (changeRequest.Content == null)
            return;

        _blobStorage.Add($"{changeRequest.Id}_Req", changeRequest.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task SaveContentOfChangeResponse(RelationshipChangeResponse changeResponse)
    {
        if (changeResponse.Content == null)
            return;

        _blobStorage.Add($"{changeResponse.Id}_Res", changeResponse.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task FillContentOfChanges(IEnumerable<RelationshipChange> changes)
    {
        await Task.WhenAll(changes.Select(FillContentOfChange).ToArray());
    }

    public async Task FillContentOfChange(RelationshipChange change)
    {
        if (change.Type == RelationshipChangeType.Creation)
        {
            var requestContent = await _blobStorage.FindAsync($"{change.Id}_Req");
            change.Request.LoadContent(requestContent);

            if (change.IsCompleted)
            {
                var responseContent = await _blobStorage.FindAsync($"{change.Id}_Res");
                change.Response!.LoadContent(responseContent);
            }
        }
    }

    public async Task FillContentOfTemplates(IEnumerable<RelationshipTemplate> templates)
    {
        await Task.WhenAll(templates.Select(FillContentOfTemplate).ToArray());
    }
}
