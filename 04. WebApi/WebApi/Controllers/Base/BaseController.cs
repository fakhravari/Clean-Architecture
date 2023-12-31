using Infrastructure.Config.Jwt.FiltersResult;
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
    }
}
