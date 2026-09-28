using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Contracts.DomainEvents;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.DomainEvents;

public static class FileOwnershipClaimedDomainEventExtensions
{
    extension(FileOwnershipClaimedDomainEvent)
    {
        public static FileOwnershipClaimedDomainEvent Create(File file, IdentityAddress oldOwnerAddress) => new()
        {
            FileId = file.Id.Value,
            OldOwnerAddress = oldOwnerAddress.Value,
            NewOwnerAddress = file.Owner.Value
        };
    }
}

public static class FileOwnershipLockedDomainEventExtensions
{
    extension(FileOwnershipLockedDomainEvent)
    {
        public static FileOwnershipLockedDomainEvent Create(File file) => new()
        {
            FileId = file.Id.Value,
            OwnerAddress = file.Owner.Value
        };
    }
}

public static class FileUploadedDomainEventExtensions
{
    extension(FileUploadedDomainEvent)
    {
        public static FileUploadedDomainEvent Create(File file) => new()
        {
            DomainEventId = $"{file.Id}/Created",
            FileId = file.Id.ToString(),
            Owner = file.Owner.ToString()
        };
    }
}
