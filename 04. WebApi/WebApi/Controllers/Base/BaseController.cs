using Microsoft.AspNetCore.Mvc;
using WebApi.Config.Utilities.Filters;

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
