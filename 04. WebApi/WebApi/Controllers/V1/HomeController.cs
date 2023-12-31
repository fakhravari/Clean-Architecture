using Application.Model;
using Infrastructure.Config.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> PublicAction(PublicActionDTO model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> PrivateAction(PrivateActionDTO model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> JwtTokenGenerate(AuthorizeDTO model)
        {
            var jwt = iwJwtService.JwtTokenGenerate(model.UserName, model.Password);

            return Ok(jwt);
        }



        /// <summary>
        /// ورژن موبایل
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("GetVersion = " + DateTime.Now.ToString());
        }
    }
}
