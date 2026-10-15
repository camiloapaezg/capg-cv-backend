using RabbitMQ.Client.Events;

namespace capg_hv_backend.Infrastructure.MessageBroker.Abstractions;

public interface IMessageBrokerClient : IDisposable
{
    Task RegisterMessageHandlerAsync(string queueName, AsyncEventHandler<BasicDeliverEventArgs> handler, CancellationToken token = default);

    Task SendMessageAsync(string queueName, byte[] body, CancellationToken token = default);
}