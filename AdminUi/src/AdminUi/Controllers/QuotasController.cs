using Enmeshed.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class QuotasController : ApiControllerBase
{
    public QuotasController(IMediator mediator) : base(mediator)
    {
    }
}
