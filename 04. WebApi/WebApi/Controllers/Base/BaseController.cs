using Localization.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base
{
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    public class BaseController : Controller
    {
        private ISharedResource? _localizer;
        protected ISharedResource Localizer => HttpContext.RequestServices.GetRequiredService<ISharedResource>();

        public BaseController()
        {

        }

        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
