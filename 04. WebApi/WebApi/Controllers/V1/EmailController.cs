using Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1;

[ApiVersion("1")]
public class EmailController : BaseController
{
    private readonly IEmailRepository _emailRepository;
    public EmailController(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string ToEmail, string Subject, string Message)
    {
        await _emailRepository.SendEmailAsync(ToEmail, Subject, Message);

        return Ok("Message sent successfully.");
    }
}