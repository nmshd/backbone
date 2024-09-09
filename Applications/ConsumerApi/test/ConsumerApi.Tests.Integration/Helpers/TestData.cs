using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class TestData
{
    public const string SOME_BASE64_STRING = "AAAA";
    public static readonly byte[] SOME_BYTES = SOME_BASE64_STRING.GetBytes();
}
