using RabbitMQ.Client.Events;

namespace capg_hv_backend.Infrastructure.MessageBroker.Abstractions;

public interface IMessageBroker : IDisposable
{
    Task SendMessage(string queueName, byte[] body, CancellationToken token = default);

    Task RegisterMessageHandler(string queueName, AsyncEventHandler<BasicDeliverEventArgs> handler, CancellationToken token = default);
}
