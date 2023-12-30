using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Config.Jwt;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        private readonly IJwtService iwJwtService;
        public HomeController(IJwtService _iwJwtService)
        {
            this.iwJwtService = _iwJwtService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendSMS(string Phone)
        {
            return Ok("Phone = " + Phone);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> SendSMS_Authorize(string Phone)
        {
            return Ok("Phone = " + Phone);
        }

        [HttpPost("[action]")]
        public IActionResult JwtTokenGenerate(string UserName, string Password)
        {
            var jwt = iwJwtService.JwtTokenGenerate(UserName, Password);

            return Ok(jwt);
        }






        /// <summary>
        /// ورژن موبایل
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("GetVersion = 44");
        }
        /// <summary>
        /// VERSION MOBILE
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetVersion2()
        {
            return Ok("GetVersion2 = 44");
        }
    }
}
