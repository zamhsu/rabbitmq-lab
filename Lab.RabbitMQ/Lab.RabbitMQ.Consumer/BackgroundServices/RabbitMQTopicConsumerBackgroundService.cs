using Lab.RabbitMQ.Consumer.Models;
using Lab.RabbitMQ.Consumer.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;

namespace Lab.RabbitMQ.Consumer.BackgroundServices;

public class RabbitMQTopicConsumerBackgroundService : BackgroundService
{
    private readonly IAdvancedBus _bus;
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly ILogger<RabbitMQTopicConsumerBackgroundService> _logger;
    private IDisposable _subscription;

    public RabbitMQTopicConsumerBackgroundService(
        IAdvancedBus bus,
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQTopicConsumerBackgroundService> logger)
    {
        _bus = bus;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("RabbitMQ Topic 消費者背景服務已啟動");
            await StartConsumingAsync(stoppingToken);

            // 保持服務運行直到取消令牌觸發
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消操作，不需要記錄錯誤
            _logger.LogInformation("RabbitMQ Topic 消費者背景服務已被取消");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ Topic 消費者背景服務發生異常");
            throw;
        }
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("開始監聽 Topic Exchange 訊息...");

        // 宣告交換機
        var exchange = await _bus.ExchangeDeclareAsync(
            name: _options.Value.TopicExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        // 宣告 Quorum Queue
        var queue = await _bus.QueueDeclareAsync(
            name: _options.Value.TopicQueueName,
            configure: config =>
            {
                config.AsDurable(true);
                config.AsExclusive(false);
                config.AsAutoDelete(false);
                config.WithArgument("x-queue-type", "quorum");
            },
            cancellationToken: cancellationToken
        );

        // 綁定 Queue 到 Exchange
        await _bus.BindAsync(exchange, queue, _options.Value.TopicRoutingKey, cancellationToken);

        // 訂閱訊息
        _subscription = _bus.Consume(queue, (body, properties, info) =>
        {
            try
            {
                // 使用 System.Text.Json 手動反序列化
                var message = System.Text.Json.JsonSerializer.Deserialize<Message>(body.Span);
                
                if (message != null)
                {
                    _logger.LogInformation("[Exchange Topic] 收到訊息: Id={Id}, Content={Content}, CreatedAt={CreatedAt}, GUID={RandomGuid}",
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

        _logger.LogInformation("RabbitMQ Topic 訊息監聽已啟動");
        
        // 返回已完成的任務，因為消費者在背景運行
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("正在停止 RabbitMQ Topic 消費者背景服務...");
        
        // 處理取消訂閱
        _subscription?.Dispose();
        _logger.LogInformation("已停止監聽 RabbitMQ Topic 訊息");
        
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _subscription?.Dispose();
        (_bus as IDisposable)?.Dispose();
        
        base.Dispose();
    }
}

