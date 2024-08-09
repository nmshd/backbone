namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class DependsOnAttribute : Attribute
{
    public DependsOnAttribute(ModuleType module, string migrationId)
    {
        Module = module;
        MigrationId = migrationId;
    }

    public ModuleType Module { get; }
    public string MigrationId { get; }
}

public enum ModuleType
{
    AdminApi,
    Challenges,
    Devices,
    Files,
    Messages,
    Quotas,
    Relationships,
    Synchronization,
    Tokens
}
