using System.Reflection;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Relationships;
public class RelationshipTests
{
    [Fact]
    public void Relationship_must_be_active_to_allow_sending_messages()
    {
        var relationship = CreateRelationship(RelationshipStatus.Pending);

        var acting = () => relationship.EnsureSendingMessagesIsAllowed(5, 5);

        //relationship.Should().NotBe(null);
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.message.relationshipToRecipientNotActive");
    }

    [Fact]
    public void Max_number_of_unrecieved_messages_cannot_be_reached_to_allow_sending_messages()
    {
        var relationship = CreateRelationship();

        var acting = () => relationship.EnsureSendingMessagesIsAllowed(5, 5);

        //relationship.Should().NotBe(null);
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.message.maxNumberOfUnreceivedMessagesReached");
    }

    #region helpers
    private static Relationship CreateRelationship()
    {
        return CreateRelationship(null, null, null, null, null);
    }

    private static Relationship CreateRelationship(RelationshipStatus status)
    {
        return CreateRelationship(null, null, null, null, status);
    }

    private static Relationship CreateRelationship(string? relationshipId = null, IdentityAddress? from = null, IdentityAddress? to = null, DateTime? createdAt = null, RelationshipStatus? status = null)
    {
        var type = typeof(Relationship);
        var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var relationship = (Relationship)constructor!.Invoke(parameters: []);

        relationshipId ??= "REL00000000000000000";
        from ??= TestDataGenerator.CreateRandomIdentityAddress();
        to ??= TestDataGenerator.CreateRandomIdentityAddress();
        createdAt ??= DateTime.UtcNow;
        status ??= RelationshipStatus.Active;

        SetBackingField(relationship, "Id", RelationshipId.Parse(relationshipId));
        SetBackingField(relationship, "From", from);
        SetBackingField(relationship, "To", to);
        SetBackingField(relationship, "CreatedAt", createdAt);
        SetBackingField(relationship, "Status", status);

        return relationship;
    }

    private static void SetBackingField(object obj, string propertyName, object value)
    {
        var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (field == null)
            throw new InvalidOperationException($"BackingField for {propertyName} not found on {obj.GetType().Name}.");

        field.SetValue(obj, value);
    }
    #endregion
}
