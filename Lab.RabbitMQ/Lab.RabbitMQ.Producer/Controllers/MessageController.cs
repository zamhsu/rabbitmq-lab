using Lab.RabbitMQ.Producer.Models;
using Lab.RabbitMQ.Producer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab.RabbitMQ.Producer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IRabbitMQTopicService _rabbitMqTopicService;
    private readonly IRabbitMQDirectService _rabbitMqDirectService;
    private readonly IRabbitMQFanoutService _rabbitMqFanoutService;

    public MessageController(
        IRabbitMQTopicService rabbitMqTopicService,
        IRabbitMQDirectService rabbitMqDirectService,
        IRabbitMQFanoutService rabbitMqFanoutService)
    {
        _rabbitMqTopicService = rabbitMqTopicService;
        _rabbitMqDirectService = rabbitMqDirectService;
        _rabbitMqFanoutService = rabbitMqFanoutService;
    }

    [HttpPost("exchange/topic")]
    public async Task<IActionResult> ExchangeTopicAsync([FromBody] Message message)
    {
        await _rabbitMqTopicService.PublishMessageAsync(message);
        return Ok(new { message = "[Exchange Topic] 訊息已成功發送到 Topic Queue", messageId = message.Id });
    }

    [HttpPost("exchange/direct")]
    public async Task<IActionResult> ExchangeDirectAsync([FromBody] Message message)
    {
        await _rabbitMqDirectService.PublishMessageAsync(message);
        return Ok(new { message = "[Exchange Direct] 訊息已成功發送到 Direct Queue", messageId = message.Id });
    }

    [HttpPost("exchange/fanout")]
    public async Task<IActionResult> ExchangeFanoutAsync([FromBody] Message message)
    {
        await _rabbitMqFanoutService.PublishMessageAsync(message);
        return Ok(new { message = "[Exchange Fanout] 訊息已成功發送到 Fanout Queue", messageId = message.Id });
    }
}