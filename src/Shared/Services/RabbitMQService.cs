using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Events;

namespace Shared.Services;

public class RabbitMQService : IMessageService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        catch (Exception ex)
        {
            // RabbitMQ não está disponível, mas não vamos falhar
            // Os serviços podem rodar sem RabbitMQ
            _connection = null;
            _channel = null;
        }
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class
    {
        if (_channel == null)
        {
            // RabbitMQ não está disponível, apenas retorna
            return;
        }

        try
        {
            // Declarar exchange se não existir
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

        }
        catch (Exception ex)
        {
            // Ignorar erros do RabbitMQ
        }
    }

    public async Task SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, Task> handler) where T : class
    {
        if (_channel == null)
        {
            // RabbitMQ não está disponível, apenas retorna
            return;
        }

        try
        {
            // Declarar exchange e queue
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message != null)
                    {
                        await handler(message);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        }
        catch (Exception ex)
        {
            // Ignorar erros do RabbitMQ
        }
    }

    public void Dispose()
    {
        try
        {
            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
            }
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            // Ignorar erros ao fechar
        }
    }
}
