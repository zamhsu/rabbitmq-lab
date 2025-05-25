using System.Text.Json;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;
using Lab.RabbitMQ.Consumer.BackgroundServices;
using Lab.RabbitMQ.Consumer.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置 RabbitMQ Options
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

// 配置 RabbitMQ 服務
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMQOptions>>();
    return RabbitHutch.CreateBus(options.Value.ConnectionString, x => 
    {
        x.Register<ISerializer>(_ => new SystemTextJsonSerializer(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }));
    }).Advanced;
});

builder.Services.AddHostedService<RabbitMQDirectConsumerBackgroundService>();
builder.Services.AddHostedService<RabbitMQTopicConsumerBackgroundService>();
builder.Services.AddHostedService<RabbitMQFanoutConsumerBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();