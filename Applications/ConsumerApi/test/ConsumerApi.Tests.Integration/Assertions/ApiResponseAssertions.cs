using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Integration.Support;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.Assertions;

public class ApiResponseAssertions<T> : ObjectAssertions<ApiResponse<T>, ApiResponseAssertions<T>>
{
    private readonly AssertionChain _assertionChain;

    public ApiResponseAssertions(ApiResponse<T> instance, AssertionChain assertionChain) : base(instance, assertionChain)
    {
        _assertionChain = assertionChain;
    }

    protected override string Identifier => "ApiResponse";

    public async Task ComplyWithSchema(string because = "", params object[] becauseArgs)
    {
        var assertion = _assertionChain
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.Result!)
            .ForCondition(result => result != null)
            .FailWith("You can't validate the JSON schema of a NULL object")
            .Then;

        if (Subject.Result != null)
        {
            var resultJson = JsonConvert.SerializeObject(Subject.Result);
            var (isValid, errors) = await JsonValidator.ValidateJsonSchema<T>(resultJson);

            assertion.ForCondition(_ => isValid)
                .FailWith($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
        }
    }

    public void BeASuccess(string because = "", params object[] becauseArgs)
    {
        _assertionChain
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(result => result.IsSuccess)
            .FailWith($"Expected response to be successful. Failed with error code: '{Subject.Error?.Code}'. Error Message: '{Subject.Error?.Message}'");
    }
}
