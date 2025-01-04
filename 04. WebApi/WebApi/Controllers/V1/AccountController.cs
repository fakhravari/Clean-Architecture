using Application.Features.Account.Commands.Login;
using Application.Features.Account.Queries.RefreshToken;
using Localization.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1;
// [HttpPost("{questionAnswerId}/quesitons")]   [FromRoute] Guid questionAnswerId
// [FromBody] ViewModel

[ApiVersion("1")]
public class AccountController : BaseController
{
    private readonly IStringLocalizer<SharedResource> localizer;

    public AccountController(IStringLocalizer<SharedResource> _localizer)
    {
        localizer = _localizer;
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

    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }
}