using Lab.RabbitMQ.Consumer.Options;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Consumer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
        return services;
    }
}