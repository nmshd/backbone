using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Mvc.ControllerAttributes;

public class ProducesErrorAttribute : ProducesResponseTypeAttribute
{
    public ProducesErrorAttribute(int statusCode) : base(typeof(HttpResponseEnvelopeError), statusCode)
    {
    }
}
