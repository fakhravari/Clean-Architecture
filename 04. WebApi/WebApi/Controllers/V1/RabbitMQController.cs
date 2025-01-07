using Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1;

[ApiVersion("1")]
public class RabbitMQController : BaseController
{
    private readonly IRabbitMQRepository _rabbitMQRepository;

    public RabbitMQController(IRabbitMQRepository rabbitMQRepository)
    {
        _rabbitMQRepository = rabbitMQRepository;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        await _rabbitMQRepository.SendMessageAsync(message);
        return Ok("Message sent successfully.");
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveMessages()
    {
        await _rabbitMQRepository.ReceiveMessagesAsync();
        return Ok("Started receiving messages.");
    }

}