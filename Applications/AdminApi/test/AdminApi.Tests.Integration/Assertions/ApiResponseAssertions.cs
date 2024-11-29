using System.Text.Json;
using Backbone.AdminApi.Tests.Integration.Support;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Backbone.AdminApi.Tests.Integration.Assertions;

public class ApiResponseAssertions<T> : ReferenceTypeAssertions<ApiResponse<T>, ApiResponseAssertions<T>>
{
    public ApiResponseAssertions(ApiResponse<T> instance) : base(instance)
    {
    }

    protected override string Identifier => "ApiResponse";

    public async Task ComplyWithSchema(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.Result!)
            .ForCondition(result => result != null)
            .FailWith("You can't validate the JSON schema of a NULL object")
            .Then;

        if (Subject.Result != null)
        {
            var resultJson = JsonSerializer.Serialize(Subject.Result);
            var (isValid, errors) = await JsonValidator.ValidateJsonSchema<T>(resultJson);

            assertion.ForCondition(_ => isValid)
                .FailWith($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
        }
    }

    public void BeASuccess(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(result => result.IsSuccess)
            .FailWith($"Expected response to be successful. Failed with error code: '{Subject.Error?.Code}'. Error Message: '{Subject.Error?.Message}'");
    }
}
