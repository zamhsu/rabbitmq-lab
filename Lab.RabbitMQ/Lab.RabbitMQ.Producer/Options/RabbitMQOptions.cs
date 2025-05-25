namespace Lab.RabbitMQ.Producer.Options;

/// <summary>
/// RabbitMQ 配置選項類別，用於管理 RabbitMQ 連接、交換機和佇列的相關設定。
/// 支援 Topic 和 Direct 兩種交換機類型的配置。
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// 定義配置節點名稱常數
    /// </summary>
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// 用於設定與 RabbitMQ 伺服器連線的字串。
    /// 該屬性接受一個符合 AMQP URI 規範的連線字串，用以建立與 RabbitMQ 的連線。
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定 Topic Exchange 的名稱。
    /// 該屬性指定使用者在 RabbitMQ 中所定義的 Topic Exchange 名稱，
    /// 並用於建立與 Exchange 進行消息傳遞的綁定。
    /// </summary>
    public string TopicExchangeName { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定主題交換器所綁定的佇列名稱。
    /// 該屬性指定了主題佇列的名稱，該佇列將接收符合特定路由鍵的訊息。
    /// </summary>
    public string TopicQueueName { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定主題交換機的路由鍵。
    /// 該屬性定義了消息在主題交換機中路由到特定佇列的規則。
    /// 可使用具名路由鍵模式進行消息的訂閱與發佈。
    /// </summary>
    public string TopicRoutingKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 用於設定 Direct Exchange 的名稱。
    /// 該屬性指定使用者在 RabbitMQ 中所定義的 Direct Exchange 名稱，
    /// 用於建立精確匹配的消息路由。
    /// </summary>
    public string DirectExchangeName { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定直接交換器所綁定的佇列名稱。
    /// 該屬性指定了直接佇列的名稱，該佇列將接收完全匹配路由鍵的訊息。
    /// </summary>
    public string DirectQueueName { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定直接交換機的路由鍵。
    /// 該屬性定義了消息在直接交換機中的精確路由規則，
    /// 只有完全匹配此路由鍵的消息才會被傳送到指定的佇列。
    /// </summary>
    public string DirectRoutingKey { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定 Fanout Exchange 的名稱。
    /// 該屬性指定使用者在 RabbitMQ 中所定義的 Fanout Exchange 名稱，
    /// 用於建立廣播消息的路由。
    /// </summary>
    public string FanoutExchangeName { get; set; } = string.Empty;

    /// <summary>
    /// 用於設定 Fanout Exchange 所綁定的佇列名稱。
    /// 該屬性指定了 Fanout 佇列的名稱，該佇列將接收所有廣播的訊息。
    /// </summary>
    public string FanoutQueueName { get; set; } = string.Empty;
}
