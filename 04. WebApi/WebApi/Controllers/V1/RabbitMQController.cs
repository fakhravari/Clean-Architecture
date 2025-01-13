using Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1;

[ApiVersion("1")]
public class RabbitMQController : BaseController
{
    private readonly IRabbitMQRepository _rabbitMQRepository;
    private readonly IEmailRepository _emailRepository;
    public RabbitMQController(IRabbitMQRepository rabbitMQRepository, IEmailRepository emailRepository)
    {
        _rabbitMQRepository = rabbitMQRepository;
        _emailRepository = emailRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string ToEmail, string Subject, string Message)
    {
        await _rabbitMQRepository.SendMessageAsync(ToEmail, Subject, Message);

        // await _emailRepository.SendEmailAsync(ToEmail, Subject, Message);

        return Ok("Message sent successfully.");
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessages()
    {
        await _rabbitMQRepository.ReceiveMessagesAsync();
        return Ok("Started receiving messages.");
    }
}