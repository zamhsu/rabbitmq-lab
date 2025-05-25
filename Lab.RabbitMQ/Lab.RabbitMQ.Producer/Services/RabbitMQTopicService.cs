using Lab.RabbitMQ.Producer.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Producer.Services;

public class RabbitMQTopicService : IRabbitMQTopicService, IDisposable
{
    private readonly IAdvancedBus _bus;
    private readonly Exchange _topicExchange;
    private readonly Queue _topicQueue;
    private readonly string _topicRoutingKey;
    private readonly string _topicExchangeName;
    private readonly string _topicQueueName;
    
    public RabbitMQTopicService(IAdvancedBus bus, IOptions<RabbitMQOptions> options)
    {
        _bus = bus;
        _topicRoutingKey = options.Value.TopicRoutingKey;
        _topicExchangeName = options.Value.TopicExchangeName;
        _topicQueueName = options.Value.TopicQueueName;

        // Topic Exchange/Queue
        _topicExchange = _bus.ExchangeDeclare(
            name: _topicExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );
        
        _topicQueue = _bus.QueueDeclare(
            name: _topicQueueName,
            configure: config =>
            {
                config.AsDurable(true);
                config.AsExclusive(false);
                config.AsAutoDelete(false);
                config.WithArgument("x-queue-type", "quorum");
            }
        );
        
        _bus.Bind(_topicExchange, _topicQueue, _topicRoutingKey);
    }
    
    public async Task PublishMessageAsync<T>(T message) where T : class
    {
        var messageBody = new Message<T>(message);
        await _bus.PublishAsync(_topicExchange, _topicRoutingKey, false, messageBody);
    }
    
    public void Dispose()
    {
        (_bus as IDisposable)?.Dispose();
    }
}