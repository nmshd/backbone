﻿using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Messages.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ExpressionTests : AbstractTestsBase
{
    #region WasExchangedBetween

    [Fact]
    public void WasExchangedBetween_with_true_assertion()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var wasExchangedBetweenSenderAndRecipient = message.EvaluateWasExchangedBetweenExpression(message.CreatedBy, message.Recipients.First().Address);
        var wasExchangedBetweenRecipientAndSender = message.EvaluateWasExchangedBetweenExpression(message.Recipients.First().Address, message.CreatedBy);

        // Assert
        wasExchangedBetweenSenderAndRecipient.Should().BeTrue();
        wasExchangedBetweenRecipientAndSender.Should().BeTrue();
    }

    [Fact]
    public void WasExchangedBetween_with_false_assertion()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var result = message.EvaluateWasExchangedBetweenExpression(message.CreatedBy, TestDataGenerator.CreateRandomIdentityAddress());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region HasParticipant

    private static readonly IdentityAddress ANONYMIZED_ADDRESS = TestDataGenerator.CreateRandomIdentityAddress();

    [Fact]
    public void HasParticipant_is_true_for_sender_when_no_relationship_is_decomposed()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_true_for_sender_when_relationship_to_at_least_one_recipient_is_not_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_sender_when_relationship_to_all_recipients_are_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, ANONYMIZED_ADDRESS);
        message.DecomposeFor(message.CreatedBy, message.Recipients.Second().Address, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasParticipant_is_true_for_recipient_when_relationship_to_sender_is_not_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_recipient_when_relationship_to_sender_is_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.Recipients.First().Address, message.CreatedBy, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}

file static class MessageExtensions
{
    public static bool EvaluateWasExchangedBetweenExpression(this Message message, IdentityAddress identityAddressOne, IdentityAddress identityAddressTwo)
    {
        var expression = Message.WasExchangedBetween(identityAddressOne, identityAddressTwo);
        return expression.Compile()(message);
    }

    public static bool EvaluateHasParticipantExpression(this Message message, IdentityAddress address)
    {
        var expression = Message.HasParticipant(address);
        return expression.Compile()(message);
    }
}
