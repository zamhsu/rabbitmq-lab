namespace Lab.RabbitMQ.Producer.Services;

public interface IRabbitMQTopicService
{
    Task PublishMessageAsync<T>(T message) where T : class;
}