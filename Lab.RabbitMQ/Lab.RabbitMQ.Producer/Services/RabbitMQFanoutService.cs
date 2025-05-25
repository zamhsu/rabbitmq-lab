using Lab.RabbitMQ.Producer.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Producer.Services;

public class RabbitMQFanoutService : IRabbitMQFanoutService, IDisposable
{
    private readonly IAdvancedBus _bus;
    private readonly Exchange _fanoutExchange;
    private readonly Queue _fanoutQueue;
    private readonly string _fanoutExchangeName;
    private readonly string _fanoutQueueName;

    public RabbitMQFanoutService(IAdvancedBus bus, IOptions<RabbitMQOptions> options)
    {
        _bus = bus;
        _fanoutExchangeName = options.Value.FanoutExchangeName;
        _fanoutQueueName = options.Value.FanoutQueueName;

        // Fanout Exchange/Queue
        _fanoutExchange = _bus.ExchangeDeclare(
            name: _fanoutExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false
        );

        _fanoutQueue = _bus.QueueDeclare(
            name: _fanoutQueueName,
            configure: config =>
            {
                config.AsDurable(true);
                config.AsExclusive(false);
                config.AsAutoDelete(false);
                config.WithArgument("x-queue-type", "quorum");
            }
        );

        _bus.Bind(_fanoutExchange, _fanoutQueue, string.Empty);
    }

    public async Task PublishMessageAsync<T>(T message) where T : class
    {
        var messageBody = new Message<T>(message);
        await _bus.PublishAsync(_fanoutExchange, string.Empty, false, messageBody);
    }

    public void Dispose()
    {
        (_bus as IDisposable)?.Dispose();
    }
}