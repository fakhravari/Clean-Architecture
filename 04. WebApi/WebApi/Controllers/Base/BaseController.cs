using Localization.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class BaseController : Controller
{
    private ISharedResource? _localizer;

    private ISender? _mediator;

    protected ISharedResource Localizer => HttpContext.RequestServices.GetRequiredService<ISharedResource>();
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}