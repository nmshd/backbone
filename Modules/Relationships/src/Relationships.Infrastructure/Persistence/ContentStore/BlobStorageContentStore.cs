using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.ContentStore;

public class BlobStorageContentStore : IContentStore
{
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public BlobStorageContentStore(IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public async Task SaveContentOfTemplate(RelationshipTemplate template)
    {
        if (template.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, template.Id, template.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task FillContentOfTemplate(RelationshipTemplate template)
    {
        var content = await _blobStorage.FindAsync(_blobOptions.RootFolder, template.Id);
        template.LoadContent(content);
    }

    public async Task SaveContentOfChangeRequest(RelationshipChangeRequest changeRequest)
    {
        if (changeRequest.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, $"{changeRequest.Id}_Req", changeRequest.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task SaveContentOfChangeResponse(RelationshipChangeResponse changeResponse)
    {
        if (changeResponse.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, $"{changeResponse.Id}_Res", changeResponse.Content);
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
            var requestContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Req");
            change.Request.LoadContent(requestContent);

            if (change.IsCompleted)
            {
                var responseContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Res");
                change.Response!.LoadContent(responseContent);
            }
        }
    }

    public async Task FillContentOfTemplates(IEnumerable<RelationshipTemplate> templates)
    {
        await Task.WhenAll(templates.Select(FillContentOfTemplate).ToArray());
    }
}
