using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Json;

public class ContractResolverWithPrivates : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var jsonProperty = base.CreateProperty(member, memberSerialization);

        jsonProperty.Writable = true;

        return jsonProperty;
    }
}
