using FluentValidation;

namespace Messages.Application.Messages.Queries.ListMessages;

// ReSharper disable once UnusedMember.Global
public class ListMessagesCommandValidator : AbstractValidator<ListMessagesCommand>
{
    public ListMessagesCommandValidator() { }
}
