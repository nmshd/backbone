using System.Diagnostics.CodeAnalysis;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class DecomposeForTests : AbstractTestsBase
{
    public static readonly IdentityAddress ANONYMIZED_ADDRESS = IdentityAddress.GetAnonymized("localhost");

    [Theory]
    [ClassData(typeof(TestDataWithAllCases))]
    public void FullTest(TestInput input, TestOutput output)
    {
        // Act
        if (output.IsSuccess)
            input.Message.DecomposeFor(input.Decomposer, input.Peer, ANONYMIZED_ADDRESS);
        else
        {
            var acting = () => input.Message.DecomposeFor(input.Decomposer, input.Peer, ANONYMIZED_ADDRESS);
            acting.ShouldThrow<DomainException>().ShouldHaveError(output.ErrorCode);
        }

        // Assert
        var recipient1 = input.Message.Recipients.First();
        var recipient2 = input.Message.Recipients.Second();

        if (output.IsSuccess)
        {
            recipient1.IsRelationshipDecomposedByRecipient.ShouldBe(output.R1_HiddenForRecipient.Value);
            recipient1.IsRelationshipDecomposedBySender.ShouldBe(output.R1_HiddenForSender.Value);

            recipient2.IsRelationshipDecomposedByRecipient.ShouldBe(output.R2_HiddenForRecipient.Value);
            recipient2.IsRelationshipDecomposedBySender.ShouldBe(output.R2_HiddenForSender.Value);

            if (output.SenderIsAnonymized.Value)
                input.Message.CreatedBy.ShouldBe(ANONYMIZED_ADDRESS);
            else
                input.Message.CreatedBy.ShouldNotBe(ANONYMIZED_ADDRESS);

            if (output.R1IsAnonymized.Value)
                recipient1.Address.ShouldBe(ANONYMIZED_ADDRESS);
            else
                recipient1.Address.ShouldNotBe(ANONYMIZED_ADDRESS);

            if (output.R2IsAnonymized.Value)
                recipient2.Address.ShouldBe(ANONYMIZED_ADDRESS);
            else
                recipient2.Address.ShouldNotBe(ANONYMIZED_ADDRESS);
        }
    }
}

public enum Participant
{
    Sender,
    Recipient1,
    Recipient2
}

public record TestInput
{
    public TestInput(int id,
        Participant decomposer,
        Participant relationshipTo,
        bool senderHasAlreadyDecomposedRelationshipWithRecipient1,
        bool recipient1HasAlreadyDecomposedRelationshipWithSender,
        bool senderHasAlreadyDecomposedRelationshipWithRecipient2,
        bool recipient2HasAlreadyDecomposedRelationshipWithSender)
    {
        Id = id;

        Message = TestData.CreateMessageWithTwoRecipients();

        Decomposer = decomposer switch
        {
            Participant.Sender => Message.CreatedBy,
            Participant.Recipient1 => Message.Recipients.First().Address,
            Participant.Recipient2 => Message.Recipients.Second().Address,
            _ => throw new Exception()
        };

        if (Decomposer == Message.CreatedBy)
        {
            Peer = relationshipTo switch
            {
                Participant.Recipient1 => Message.Recipients.First().Address,
                Participant.Recipient2 => Message.Recipients.Second().Address,
                _ => throw new Exception()
            };
        }
        else
            Peer = Message.CreatedBy;

        if (recipient1HasAlreadyDecomposedRelationshipWithSender)
            Message.DecomposeFor(Message.Recipients.First().Address, Message.CreatedBy, DecomposeForTests.ANONYMIZED_ADDRESS);
        if (recipient2HasAlreadyDecomposedRelationshipWithSender)
            Message.DecomposeFor(Message.Recipients.Second().Address, Message.CreatedBy, DecomposeForTests.ANONYMIZED_ADDRESS);
        if (senderHasAlreadyDecomposedRelationshipWithRecipient1)
            Message.DecomposeFor(Message.CreatedBy, Message.Recipients.First().Address, DecomposeForTests.ANONYMIZED_ADDRESS);
        if (senderHasAlreadyDecomposedRelationshipWithRecipient2)
            Message.DecomposeFor(Message.CreatedBy, Message.Recipients.Second().Address, DecomposeForTests.ANONYMIZED_ADDRESS);
    }


    public int Id { get; init; }
    public Message Message { get; }
    public IdentityAddress Decomposer { get; }
    public IdentityAddress Peer { get; }
}

// ReSharper disable InconsistentNaming
public record TestOutput
{
    public TestOutput(bool r1_HiddenForSender, bool r1_HiddenForRecipient, bool r2_HiddenForSender, bool r2_HiddenForRecipient, bool senderIsAnonymized, bool r1IsAnonymized, bool r2IsAnonymized)
    {
        R1_HiddenForSender = r1_HiddenForSender;
        R1_HiddenForRecipient = r1_HiddenForRecipient;
        R2_HiddenForSender = r2_HiddenForSender;
        R2_HiddenForRecipient = r2_HiddenForRecipient;
        SenderIsAnonymized = senderIsAnonymized;
        R1IsAnonymized = r1IsAnonymized;
        R2IsAnonymized = r2IsAnonymized;
    }

    public TestOutput(string errorCode)
    {
        ErrorCode = errorCode;
    }


    public string? ErrorCode { get; set; }

    [MemberNotNullWhen(false, nameof(ErrorCode))]
    [MemberNotNullWhen(true, nameof(R1_HiddenForSender))]
    [MemberNotNullWhen(true, nameof(R1_HiddenForRecipient))]
    [MemberNotNullWhen(true, nameof(R2_HiddenForSender))]
    [MemberNotNullWhen(true, nameof(R2_HiddenForRecipient))]
    [MemberNotNullWhen(true, nameof(SenderIsAnonymized))]
    [MemberNotNullWhen(true, nameof(R1IsAnonymized))]
    [MemberNotNullWhen(true, nameof(R2IsAnonymized))]
    public bool IsSuccess => ErrorCode == null;

    public bool? R1_HiddenForSender { get; init; }
    public bool? R1_HiddenForRecipient { get; init; }
    public bool? R2_HiddenForSender { get; init; }
    public bool? R2_HiddenForRecipient { get; init; }
    public bool? SenderIsAnonymized { get; init; }
    public bool? R1IsAnonymized { get; init; }
    public bool? R2IsAnonymized { get; init; }
}
// ReSharper restore InconsistentNaming

public class TestDataWithAllCases : TheoryData<TestInput, TestOutput>
{
    public TestDataWithAllCases()
    {
        Add(new TestInput(0, Participant.Sender, Participant.Recipient1, false, false, false, false), new TestOutput(true, false, false, false, false, false, false));
        Add(new TestInput(1, Participant.Recipient1, Participant.Recipient1, false, false, false, false), new TestOutput(false, true, false, false, false, false, false));
        Add(new TestInput(2, Participant.Sender, Participant.Recipient2, false, false, false, false), new TestOutput(false, false, true, false, false, false, false));
        Add(new TestInput(3, Participant.Recipient2, Participant.Recipient2, false, false, false, false), new TestOutput(false, false, false, true, false, false, false));
        Add(new TestInput(4, Participant.Recipient1, Participant.Recipient1, true, false, false, false), new TestOutput(true, true, false, false, false, true, false));
        Add(new TestInput(5, Participant.Sender, Participant.Recipient2, true, false, false, false), new TestOutput(true, false, true, false, false, false, false));
        Add(new TestInput(6, Participant.Recipient2, Participant.Recipient2, true, false, false, false), new TestOutput(true, false, false, true, false, false, false));
        Add(new TestInput(7, Participant.Sender, Participant.Recipient1, false, false, true, false), new TestOutput(true, false, true, false, false, false, false));
        Add(new TestInput(8, Participant.Recipient1, Participant.Recipient1, false, false, true, false), new TestOutput(false, true, true, false, false, false, false));
        Add(new TestInput(9, Participant.Recipient2, Participant.Recipient2, false, false, true, false), new TestOutput(false, false, true, true, false, false, true));
        Add(new TestInput(10, Participant.Recipient1, Participant.Recipient1, true, false, true, false), new TestOutput(true, true, true, false, false, true, false));
        Add(new TestInput(11, Participant.Recipient2, Participant.Recipient2, true, false, true, false), new TestOutput(true, false, true, true, false, false, true));
        Add(new TestInput(12, Participant.Sender, Participant.Recipient1, false, true, false, false), new TestOutput(true, true, false, false, false, true, false));
        Add(new TestInput(13, Participant.Sender, Participant.Recipient2, false, true, false, false), new TestOutput(false, true, true, false, false, false, false));
        Add(new TestInput(14, Participant.Recipient2, Participant.Recipient2, false, true, false, false), new TestOutput(false, true, false, true, false, false, false));
        Add(new TestInput(15, Participant.Sender, Participant.Recipient2, true, true, false, false), new TestOutput(true, true, true, false, false, true, false));
        Add(new TestInput(16, Participant.Recipient2, Participant.Recipient2, true, true, false, false), new TestOutput(true, true, false, true, false, true, false));
        Add(new TestInput(17, Participant.Sender, Participant.Recipient1, false, true, true, false), new TestOutput(true, true, true, false, false, true, false));
        Add(new TestInput(18, Participant.Recipient2, Participant.Recipient2, false, true, true, false), new TestOutput(false, true, true, true, false, false, true));
        Add(new TestInput(19, Participant.Recipient2, Participant.Recipient2, true, true, true, false), new TestOutput(true, true, true, true, true, true, true));
        Add(new TestInput(20, Participant.Sender, Participant.Recipient1, false, false, false, true), new TestOutput(true, false, false, true, false, false, false));
        Add(new TestInput(21, Participant.Recipient1, Participant.Recipient1, false, false, false, true), new TestOutput(false, true, false, true, false, false, false));
        Add(new TestInput(22, Participant.Sender, Participant.Recipient2, false, false, false, true), new TestOutput(false, false, true, true, false, false, true));
        Add(new TestInput(23, Participant.Recipient1, Participant.Recipient1, true, false, false, true), new TestOutput(true, true, false, true, false, true, false));
        Add(new TestInput(24, Participant.Sender, Participant.Recipient2, true, false, false, true), new TestOutput(true, false, true, true, false, false, true));
        Add(new TestInput(25, Participant.Sender, Participant.Recipient1, false, false, true, true), new TestOutput(true, false, true, true, false, false, true));
        Add(new TestInput(26, Participant.Recipient1, Participant.Recipient1, false, false, true, true), new TestOutput(false, true, true, true, false, false, true));
        Add(new TestInput(27, Participant.Recipient1, Participant.Recipient1, true, false, true, true), new TestOutput(true, true, true, true, true, true, true));
        Add(new TestInput(28, Participant.Sender, Participant.Recipient1, false, true, false, true), new TestOutput(true, true, false, true, false, true, false));
        Add(new TestInput(29, Participant.Sender, Participant.Recipient2, false, true, false, true), new TestOutput(false, true, true, true, false, false, true));
        Add(new TestInput(30, Participant.Sender, Participant.Recipient2, true, true, false, true), new TestOutput(true, true, true, true, true, true, true));
        Add(new TestInput(31, Participant.Sender, Participant.Recipient1, false, true, true, true), new TestOutput(true, true, true, true, true, true, true));
        Add(new TestInput(32, Participant.Sender, Participant.Recipient1, true, false, false, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(33, Participant.Sender, Participant.Recipient2, false, false, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(34, Participant.Sender, Participant.Recipient1, true, false, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(35, Participant.Sender, Participant.Recipient2, true, false, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(36, Participant.Recipient1, Participant.Recipient1, false, true, false, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(37, Participant.Sender, Participant.Recipient1, true, true, false, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(38, Participant.Recipient1, Participant.Recipient1, true, true, false, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(39, Participant.Recipient1, Participant.Recipient1, false, true, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(40, Participant.Sender, Participant.Recipient2, false, true, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(41, Participant.Sender, Participant.Recipient1, true, true, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(42, Participant.Recipient1, Participant.Recipient1, true, true, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(43, Participant.Sender, Participant.Recipient2, true, true, true, false), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(44, Participant.Recipient2, Participant.Recipient2, false, false, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(45, Participant.Sender, Participant.Recipient1, true, false, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(46, Participant.Recipient2, Participant.Recipient2, true, false, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(47, Participant.Sender, Participant.Recipient2, false, false, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(48, Participant.Recipient2, Participant.Recipient2, false, false, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(49, Participant.Sender, Participant.Recipient1, true, false, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(50, Participant.Sender, Participant.Recipient2, true, false, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(51, Participant.Recipient2, Participant.Recipient2, true, false, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(52, Participant.Recipient1, Participant.Recipient1, false, true, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(53, Participant.Recipient2, Participant.Recipient2, false, true, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(54, Participant.Sender, Participant.Recipient1, true, true, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(55, Participant.Recipient1, Participant.Recipient1, true, true, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(56, Participant.Recipient2, Participant.Recipient2, true, true, false, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(57, Participant.Recipient1, Participant.Recipient1, false, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(58, Participant.Sender, Participant.Recipient2, false, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(59, Participant.Recipient2, Participant.Recipient2, false, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(60, Participant.Sender, Participant.Recipient1, true, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(61, Participant.Recipient1, Participant.Recipient1, true, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(62, Participant.Sender, Participant.Recipient2, true, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
        Add(new TestInput(63, Participant.Recipient2, Participant.Recipient2, true, true, true, true), new TestOutput("error.platform.validation.message.unableToDecompose"));
    }
}
