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
    }
}
