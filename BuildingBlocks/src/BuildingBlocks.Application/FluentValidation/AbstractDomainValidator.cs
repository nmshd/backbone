using Enmeshed.BuildingBlocks.Domain.Errors;
using FluentValidation;
using FluentValidation.Results;

namespace Enmeshed.BuildingBlocks.Application.FluentValidation;

public abstract class AbstractDomainValidator<T> : AbstractValidator<T>
{
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        var validationResult = base.Validate(context);
        var domainErrorValidationFailures = ValidateAgainstDomainModel(context.InstanceToValidate);

        foreach (var domainErrorValidationFailure in domainErrorValidationFailures.Where(e => e.Error != null))
        {
            validationResult.Errors.Add(new ValidationFailure(context.PropertyName,
                domainErrorValidationFailure.Error!.Message, domainErrorValidationFailure.PropertyName)
            { ErrorCode = domainErrorValidationFailure.Error.Code });
        }

        return validationResult;
    }

    protected abstract IEnumerable<(DomainError? Error, string PropertyName)> ValidateAgainstDomainModel(T value);
}