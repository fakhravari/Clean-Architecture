using Microsoft.AspNetCore.Mvc;
using WebApi.Config.Jwt;
using WebApi.Controllers.Base;
using static WebApi.Config.Swagger.SwaggerConfiguration;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        private readonly IJwtService iwJwtService;
        public HomeController(IJwtService _iwJwtService)
        {
            this.iwJwtService = _iwJwtService;
        }

        [AllowAnonymousAPI]
        [HttpPost("[action]")]
        public async Task<IActionResult> SendSMS(string Phone)
        {
            return Ok("Phone = " + Phone);
        }

        [AuthorizeAPI]
        [HttpPost("[action]")]
        public async Task<IActionResult> SendSMS_Authorize(string Phone)
        {
            return Ok("Phone = " + Phone);
        }

        [AllowAnonymousAPI]
        [HttpPost("[action]")]
        public IActionResult JwtTokenGenerate(string UserName, string Password)
        {
            var jwt = iwJwtService.Generate(UserName, Password);

            return Ok(jwt);
        }
    }
}
