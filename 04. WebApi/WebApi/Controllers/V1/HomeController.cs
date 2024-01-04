using Application.Features.Account.Commands.Login;
using Application.Services.Jwt;
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
        public async Task<IActionResult> PublicAction(string model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> PrivateAction(string model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> Login(LoginCommand model)
        //{
        //    var jwt = iwJwtService.JwtTokenGenerate(model);

        //    return Ok(jwt);
        //}



        /// <summary>
        /// ورژن موبایل
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("GetVersion = " + DateTime.Now.ToString());
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
