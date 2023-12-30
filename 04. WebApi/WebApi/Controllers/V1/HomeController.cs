using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        public HomeController()
        {

        }

        [HttpPost("SendSMS")]
        public async Task<IActionResult> SendSMS(string Phone)
        {
            return Ok("Phone = " + Phone);
        }


        [Authorize]
        [HttpPost("SendSMS_Authorize")]
        public async Task<IActionResult> SendSMS_Authorize(string Phone)
        {
            return Ok("Phone = " + Phone);
        }
    }
}
