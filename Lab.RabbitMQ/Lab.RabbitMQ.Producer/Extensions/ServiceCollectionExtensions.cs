using Lab.RabbitMQ.Producer.Options;

namespace Lab.RabbitMQ.Producer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(
            configuration.GetSection(RabbitMQOptions.SectionName));

        return services;
    }
}