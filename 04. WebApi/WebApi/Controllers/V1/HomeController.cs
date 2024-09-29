using Application.Features.Account.Commands.Login;
using Localization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Controllers.Base;
using WebApi.Model;

namespace WebApi.Controllers.V1
{
    // [HttpPost("{questionAnswerId}/quesitons")]   [FromRoute] Guid questionAnswerId
    // [FromBody] ViewModel


    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        private readonly IStringLocalizer<SharedResource> localizer;
        public HomeController(IStringLocalizer<SharedResource> _localizer)
        {
            localizer = _localizer;
        }

        [HttpPost]
        public async Task<IActionResult> PublicAction([FromBody] PublicActionDto model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PrivateAction([FromBody] PrivateActionDto model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }


        /// <summary>
        /// ورژن موبایل
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("GetVersion = " + DateTime.Now);
        }

        [SwaggerOperation(Description = "توضیحات متد اجرایی", Summary = "ورود به اپلیکیشن")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;
            //var uiCulture = rqf.RequestCulture.UICulture;
            var translation2 = Localizer.CheckTheInputValues;
            var tttt = localizer["Check_The_Input_Values"];
            var result = await Mediator.Send(command);
            
            return Ok(result);
        }
    }
}
