namespace Lab.RabbitMQ.Producer.Services;

/// <summary>
/// 提供功能以將訊息發佈至 RabbitMQ 的 Direct Exchange。
/// </summary>
public interface IRabbitMQDirectService
{
    /// <summary>
    /// 發佈訊息至指定的 Direct Exchange。
    /// </summary>
    /// <typeparam name="T">訊息的類型。</typeparam>
    /// <param name="message">要發佈的訊息，必須為引用類型。</param>
    /// <returns>代表非同步操作的 Task。</returns>
    Task PublishMessageAsync<T>(T message) where T : class;
}