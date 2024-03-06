using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi;

public class MessagesDbContextSeeder : IDbSeeder<MessagesDbContext>
{
    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;

    public MessagesDbContextSeeder(IServiceProvider serviceProvider)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()?.Value.RootFolder;
    }

    public async Task SeedAsync(MessagesDbContext context)
    {
        await FillBodyColumnsFromBlobStorage(context);
    }

    private async Task FillBodyColumnsFromBlobStorage(MessagesDbContext context)
    {
        // _blobRootFolder is null when blob storage configuration is not provided, meaning the content of database entries should not be loaded from blob storage
        if (_blobRootFolder == null)
            return;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var messagesWithMissingBody = await context.Messages.Where(m => m.Body == null).ToListAsync();

        foreach (var message in messagesWithMissingBody)
        {
            try
            {
                var blobMessageBody = await _blobStorage!.FindAsync(_blobRootFolder, message.Id);
                message.LoadBody(blobMessageBody);
                context.Messages.Update(message);
            }
            catch (NotFoundException)
            {
                // due to missing validation, it was possible to create a relationship template without request content;
                // therefore we cannot tell whether this exception is an error or not
            }
        }

        await context.SaveChangesAsync();
    }
}
