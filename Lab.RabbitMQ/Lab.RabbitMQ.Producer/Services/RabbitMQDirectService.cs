using Lab.RabbitMQ.Producer.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Producer.Services;

/// <summary>
/// 提供功能以將訊息發佈至 RabbitMQ 的 Direct Exchange。
/// </summary>
public class RabbitMQDirectService : IRabbitMQDirectService, IDisposable
{
    private readonly IAdvancedBus _bus;

    // Direct exchange/queue
    private readonly Exchange _directExchange;
    private readonly Queue _directQueue;
    private readonly string _directRoutingKey;
    private readonly string _directExchangeName;
    private readonly string _directQueueName;

    public RabbitMQDirectService(IAdvancedBus bus, IOptions<RabbitMQOptions> options)
    {
        _bus = bus;
        _directRoutingKey = options.Value.DirectRoutingKey;
        _directExchangeName = options.Value.DirectExchangeName;
        _directQueueName = options.Value.DirectQueueName;

        // Direct Exchange/Queue
        _directExchange = _bus.ExchangeDeclare(
            name: _directExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false
        );
        
        _directQueue = _bus.QueueDeclare(
            name: _directQueueName,
            configure: config =>
            {
                config.AsDurable(true);
                config.AsExclusive(false);
                config.AsAutoDelete(false);
                config.WithArgument("x-queue-type", "quorum");
            }
        );
        
        _bus.Bind(_directExchange, _directQueue, _directRoutingKey);
    }

    /// <summary>
    /// 發佈訊息至指定的 Direct Exchange。
    /// </summary>
    /// <typeparam name="T">訊息的類型。</typeparam>
    /// <param name="message">要發佈的訊息，必須為引用類型。</param>
    /// <returns>代表非同步操作的 Task。</returns>
    public async Task PublishMessageAsync<T>(T message) where T : class
    {
        var messageBody = new Message<T>(message);
        await _bus.PublishAsync(_directExchange, _directRoutingKey, false, messageBody);
    }

    public void Dispose()
    {
        (_bus as IDisposable)?.Dispose();
    }
}
