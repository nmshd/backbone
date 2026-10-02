using Backbone.BuildingBlocks.API.OpenApi;
using FakeItEasy;

namespace Backbone.BuildingBlocks.API.Tests.OpenApi;

public class OpenApiSchemaNamesTests : AbstractTestsBase
{
    [Theory]
    [InlineData("Command", "CreateEntityRequest")]
    [InlineData("Query", "CreateEntityRequest")]
    [InlineData("Response", "CreateEntityResponse")]
    [InlineData("RequestBody", "CreateEntityRequest")]
    [InlineData("MetadataDTO", "Metadata")]
    public void Slice_types_use_the_use_case_name(string typeName, string expected)
    {
        var type = CreateType("Backbone.Modules.TestModule.Module.Features.Entities.CreateEntity", typeName);

        new OpenApiSchemaNames().GetSchemaId(type).ShouldBe(expected);
    }

    [Theory]
    [InlineData("Backbone.Modules.TestModule.Domain.Features.CreateEntity", "Response", "Response")]
    [InlineData("Backbone.BuildingBlocks.API", "Response", "Response")]
    [InlineData("Backbone.Modules.TestModule.Module.Features.Entities.CreateEntity", "ExistingCommand", "ExistingRequest")]
    public void Other_types_keep_the_existing_naming(string ns, string typeName, string expected)
    {
        new OpenApiSchemaNames().GetSchemaId(CreateType(ns, typeName)).ShouldBe(expected);
    }

    [Fact]
    public void Historical_names_override_the_convention()
    {
        var type = CreateType("Backbone.Modules.TestModule.Module.Features.Entities.ClaimEntity", "RequestBody");
        var names = new OpenApiSchemaNames();
        names.Overrides[type] = "ClaimRequest";

        names.GetSchemaId(type).ShouldBe("ClaimRequest");
    }

    [Theory]
    [InlineData(typeof(HttpResponseEnvelopeResult<string>), "ResponseWrapper_String")]
    [InlineData(typeof(PagedHttpResponseEnvelope<string>), "PagedHttpResponseEnvelope_String")]
    [InlineData(typeof(Dictionary<string, List<int>>), "Dictionary_String_List_Int32")]
    public void Generic_types_keep_the_existing_wrapper_names(Type type, string expected)
    {
        new OpenApiSchemaNames().GetSchemaId(type).ShouldBe(expected);
    }

    [Fact]
    public void Generic_arguments_use_historical_names()
    {
        var names = new OpenApiSchemaNames();
        names.Overrides[typeof(string)] = "HistoricalResponse";

        names.GetSchemaId(typeof(HttpResponseEnvelopeResult<string>)).ShouldBe("ResponseWrapper_HistoricalResponse");
    }

    private static Type CreateType(string ns, string name)
    {
        var type = A.Fake<Type>();
        A.CallTo(() => type.Namespace).Returns(ns);
        A.CallTo(() => type.Name).Returns(name);
        return type;
    }
}
