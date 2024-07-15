// using FluentAssertions;
// using Xunit;
// using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;
//
// namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
//
// public class GetPeerTests
// {
//     [Fact]
//     public void Returns_from_if_to_is_passed()
//     {
//         // Arrange
//         var relationship = CreatePendingRelationship();
//
//         // Act
//         var peer = relationship.GetPeer(relationship.From);
//
//         // Assert
//         peer.Should().Be(relationship.To);
//     }
//
//     [Fact]
//     public void Returns_to_if_from_is_passed()
//     {
//         // Arrange
//         var relationship = CreatePendingRelationship();
//
//         // Act
//         var peer = relationship.GetPeer(relationship.To);
//
//         // Assert
//         peer.Should().Be(relationship.From);
//     }
// }


