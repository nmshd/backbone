using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Integration.Support;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.Assertions;
public class IResponseAssertions : ObjectAssertions<IResponse, IResponseAssertions>
{
    public IResponseAssertions(IResponse instance) : base(instance) { }

    public void BeASuccess(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(result => result.IsSuccess)
            .FailWith($"Expected response to be successful. Failed with error code: '{Subject.Error?.Code}'. Error Message: '{Subject.Error?.Message}'");
    }

    public async Task ComplyWithSchema(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject!)
            .ForCondition(result => result != null)
            .FailWith("You can't validate the JSON schema of a NULL object")
            .Then;

        if (Subject != null)
        {
            var type = Subject.GetType().GetGenericArguments()[0];

            var resultJson = JsonConvert.SerializeObject(Subject);

            var validateMethod = typeof(JsonValidator)
                .GetMethod("ValidateJsonSchema")!
                .MakeGenericMethod(type);

            dynamic task = validateMethod.Invoke(null, [resultJson])!;
            var result = await task;

            bool isValid = result.Item1;
            var errors = result.Item2 as IList<string>;

            if (errors != null)
                assertion.ForCondition(_ => isValid)
                    .FailWith($"Response content does not comply with the {type.FullName} schema: {string.Join(", ", errors)}");
        }
    }


}
