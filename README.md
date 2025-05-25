# rabbitmq-lab
RabbitMQ 實驗

## RabbitMQ
使用 Docker 架設 4.1.0 版的 RabbitMQ，由 3 個節點組成叢集。

架設方式參考：
- https://blog.yowko.com/docker-compose-rabbitmq-cluster/
- https://github.com/serkodev/rabbitmq-cluster-docker/tree/master

## Application
使用 ASP.NET Core 8 作為 Producer 和 Consumer 的範例。

### Lab.RabbitMQ.Producer
使用 .NET 8 WebAPI 來發送訊息。使用 EasyNetQ 的 `AdvancedBus` 功能。

#### 套件
- EasyNetQ 7.8.0
- EasyNetQ.Serialization.SystemTextJson 7.8.0

#### Web API
- Direct: /api/message/exchange/direct
- Topic: /api/message/exchange/topic
- Fanout: /api/message/exchange/fanout

### Lab.RabbitMQ.Consumer
使用 .NET 8 WebAPI 的 `BackgroundService` 在背景接收訊息。使用 EasyNetQ 的 `AdvancedBus` 功能。

#### 套件
- EasyNetQ 7.8.0
- EasyNetQ.Serialization.SystemTextJson 7.8.0

#### Background Service
- Direct: RabbitMQDirectConsumerBackgroundService
- Topic: RabbitMQTopicConsumerBackgroundService
- Fanout: RabbitMQFanoutConsumerBackgroundService