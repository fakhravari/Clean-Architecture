using Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        private ISharedViewLocalizer? _localizer;
        protected ISharedViewLocalizer Localizer => HttpContext.RequestServices.GetRequiredService<ISharedViewLocalizer>();

        public BaseController()
        {

        }

        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
