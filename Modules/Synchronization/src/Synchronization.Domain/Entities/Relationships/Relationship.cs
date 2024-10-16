using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Domain.Entities.Relationships;

public class Relationship
{
    public RelationshipId Id { get; } = null!;
    public IdentityAddress From { get; } = null!;
    public IdentityAddress To { get; } = null!;
    public RelationshipStatus Status { get; }

    public bool IsBetween(IdentityAddress identity1, IdentityAddress identity2)
    {
        return From == identity1 && To == identity2 ||
               From == identity2 && To == identity1;
    }
}

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60,
    ReadyForDeletion = 70
}

public record RelationshipId(string Value) : StronglyTypedId(Value)
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "REL";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public static RelationshipId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(RelationshipId)}.");

        return new RelationshipId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static RelationshipId New()
    {
        var challengeIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipId(PREFIX + challengeIdAsString);
    }

    public class RelationshipIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue) ? Parse(stringValue) : base.ConvertFrom(context, culture, value);
        }
    }
}
