using Microsoft.AspNetCore.Mvc;

namespace ConsumerAPI.Mvc.ControllerAttributes;

public class ProducesErrorAttribute : ProducesResponseTypeAttribute
{
    public ProducesErrorAttribute(int statusCode) : base(typeof(HttpResponseEnvelopeError), statusCode)
    {
    }
}