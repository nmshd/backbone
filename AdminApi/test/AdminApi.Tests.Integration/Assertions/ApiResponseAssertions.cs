using Backbone.AdminApi.Tests.Integration.Validators;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json;

namespace Backbone.AdminApi.Tests.Integration.Assertions;

public class ApiResponseAssertions<T> : ReferenceTypeAssertions<ApiResponse<T>, ApiResponseAssertions<T>>
{
    public ApiResponseAssertions(ApiResponse<T> instance) : base(instance)
    {
    }

    protected override string Identifier => "ApiResponse";

    public void ComplyWithSchema()
    {
        IList<string> errors = [];
        Execute.Assertion
            .Given(() => Subject.Result!)
            .ForCondition(result => result != null)
            .FailWith("You can't validate the JSON schema of a NULL object")
            .Then
            .ForCondition(result => JsonValidators.ValidateJsonSchema<T>(JsonConvert.SerializeObject(result), out errors))
            .FailWith($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
