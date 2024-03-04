using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Json;

public class ContractResolverWithPrivates : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        prop.Writable = true;

        if (prop.Writable) return prop;

        var property = member as PropertyInfo;
        if (property != null)
        {
            var hasPrivateSetter = property.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
        }

        return prop;
    }
}
