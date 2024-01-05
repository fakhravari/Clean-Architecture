using Application.Features.Account.Commands.Login;
using Application.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Model;
using WebApi.Services;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        private readonly IJwtService iwJwtService;

        private readonly ISharedViewLocalizer _localizer;


        public HomeController(IJwtService _iwJwtService, ISharedViewLocalizer localizer)
        {
            this.iwJwtService = _iwJwtService;
            this._localizer = localizer;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PublicAction(PublicActionDto model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> PrivateAction(PrivateActionDto model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
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


        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var trans = _localizer.Locale("String1");


            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
