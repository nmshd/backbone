using Admin.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OpenIddict.Validation.AspNetCore;

namespace Admin.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class QuotasController : ApiControllerBase
{
    public QuotasController(IMediator mediator) : base(mediator)
    {
    }
}
