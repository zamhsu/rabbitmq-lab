namespace Lab.RabbitMQ.Producer.Models;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public string RandomGuid { get; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}