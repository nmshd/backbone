using Backbone.BuildingBlocks.API.Extensions;
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
        if (_blobRootFolder == null)
            return;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var messages = await context.Messages.Where(m => m.Body == null).ToListAsync();

        foreach (var message in messages)
        {
            message.LoadBody(await _blobStorage.FindAsync(_blobRootFolder, message.Id));
            context.Messages.Update(message);
        }
        
        await context.SaveChangesAsync();
    }
}
