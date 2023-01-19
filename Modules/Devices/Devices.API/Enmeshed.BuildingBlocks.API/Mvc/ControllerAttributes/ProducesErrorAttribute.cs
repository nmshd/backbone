using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes
{
    public class ProducesErrorAttribute : ProducesResponseTypeAttribute
    {
        public ProducesErrorAttribute(int statusCode) : base(typeof(HttpResponseEnvelopeError), statusCode) { }
    }
}
