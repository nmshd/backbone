namespace Backbone.BuildingBlocks.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class JsonSchemaNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
