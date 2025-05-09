namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class RegexFor
{
    public const string SINGLE_THING = "([a-zA-Z0-9]+)";
    public const string OPTIONAL_SINGLE_THING = "([a-zA-Z0-9-]+)";
    public const string LIST_OF_THINGS = "([a-zA-Z0-9, ]+)";
    public const string URL = @"([a-zA-Z0-9/.:]+)";
}
