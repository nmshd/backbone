using Microsoft.AspNetCore.Mvc;

namespace ConsumerApi.Mvc.ControllerAttributes;

public class ProducesErrorAttribute : ProducesResponseTypeAttribute
{
    public ProducesErrorAttribute(int statusCode) : base(typeof(HttpResponseEnvelopeError), statusCode)
    {
    }
}