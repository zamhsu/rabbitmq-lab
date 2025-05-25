using Lab.RabbitMQ.Consumer.Models;
using Lab.RabbitMQ.Consumer.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Consumer.BackgroundServices;

/// <summary>
/// RabbitMQ Fanout Exchange 消費者背景服務
/// </summary>
public class RabbitMQFanoutConsumerBackgroundService : BackgroundService
{
    private readonly IAdvancedBus _bus;
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly ILogger<RabbitMQFanoutConsumerBackgroundService> _logger;
    private IDisposable _subscription;

    public RabbitMQFanoutConsumerBackgroundService(
        IAdvancedBus bus,
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQFanoutConsumerBackgroundService> logger)
    {
        _bus = bus;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("RabbitMQ Fanout 消費者背景服務已啟動");
            await StartConsumingAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ Fanout 消費者背景服務已被取消");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ Fanout 消費者背景服務發生異常");
            throw;
        }
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("開始監聽 Fanout Exchange 訊息...");

        // 宣告 Fanout 交換機
        var exchange = await _bus.ExchangeDeclareAsync(
            name: _options.Value.FanoutExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        // 宣告 Queue（不一定要 Quorum，可依需求調整）
        var queue = await _bus.QueueDeclareAsync(
            name: _options.Value.FanoutQueueName,
            configure: config =>
            {
                config.AsDurable(true);
                config.AsExclusive(false);
                config.AsAutoDelete(false);
            },
            cancellationToken: cancellationToken
        );

        // 綁定 Queue 到 Fanout Exchange（Fanout 無需 RoutingKey，可空字串）
        await _bus.BindAsync(exchange, queue, string.Empty, cancellationToken);

        // 訂閱消息
        _subscription = _bus.Consume(queue, (body, properties, info) =>
        {
            try
            {
                var message = System.Text.Json.JsonSerializer.Deserialize<Message>(body.Span);
                if (message != null)
                {
                    _logger.LogInformation("[Exchange Fanout] 收到訊息: Id={Id}, Content={Content}, CreatedAt={CreatedAt}, GUID={RandomGuid}",
                        message.Id, message.Content, message.CreatedAt, message.RandomGuid);
                }
                else
                {
                    _logger.LogWarning("收到訊息但反序列化結果為空");
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理訊息時發生錯誤");
                return Task.CompletedTask;
            }
        });

        _logger.LogInformation("RabbitMQ Fanout 訊息監聽已啟動");
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("正在停止 RabbitMQ Fanout 消費者背景服務...");
        _subscription?.Dispose();
        _logger.LogInformation("已停止監聽 RabbitMQ Fanout 訊息");
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _subscription?.Dispose();
        (_bus as IDisposable)?.Dispose();
        base.Dispose();
    }
}
