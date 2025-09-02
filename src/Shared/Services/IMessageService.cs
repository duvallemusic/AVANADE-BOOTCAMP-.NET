using Shared.Events;

namespace Shared.Services;

public interface IMessageService
{
    Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class;
    Task SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, Task> handler) where T : class;
    void Dispose();
}
