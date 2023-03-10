using Microsoft.AspNetCore.Mvc;

namespace Backbone.API.Mvc.ControllerAttributes;

public class ProducesErrorAttribute : ProducesResponseTypeAttribute
{
    public ProducesErrorAttribute(int statusCode) : base(typeof(HttpResponseEnvelopeError), statusCode)
    {
    }
}