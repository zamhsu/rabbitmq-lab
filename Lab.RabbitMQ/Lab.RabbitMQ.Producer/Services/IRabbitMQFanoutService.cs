namespace Lab.RabbitMQ.Producer.Services;

public interface IRabbitMQFanoutService
{
    Task PublishMessageAsync<T>(T message) where T : class;
}