using Infrastructure.Config.Jwt.FiltersResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class BaseController : Controller
    {
        public BaseController()
        {
           
        }
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
