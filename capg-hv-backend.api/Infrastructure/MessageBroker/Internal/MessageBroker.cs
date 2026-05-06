using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace capg_hv_backend.Infrastructure.MessageBroker.Internal;

public sealed class MessageBroker(IOptions<MessageBrokerOptions> options) : IMessageBroker
{
    private readonly MessageBrokerOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    private IChannel? _channel = null;

    private IConnection? _connection = null;

    private AsyncEventingBasicConsumer? _consumer = null;

    private AsyncEventHandler<BasicDeliverEventArgs>? _onMessageReceived = null;

    public void Dispose()
    {
        _consumer?.ReceivedAsync -= _onMessageReceived;
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async Task RegisterMessageHandler(string queueName, AsyncEventHandler<BasicDeliverEventArgs> handler, CancellationToken token = default)
    {
        _channel = await CreateChannel("capg-api-receiver", token);

        // Creates the queue if it does not exist.
        await CreateQueue(_channel, queueName, token);

        // Creates the consumer and assigns the handler.
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _onMessageReceived = handler;
        _consumer.ReceivedAsync += _onMessageReceived;

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: true,
            consumer: _consumer,
            cancellationToken: token);
    }

    public async Task SendMessage(string queueName, byte[] body, CancellationToken token = default)
    {
        using IChannel channel = await CreateChannel("capg-api-sender", token);

        // Creates the queue if it does not exist.
        await CreateQueue(channel, queueName, token);

        // Sends the message
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body,
            cancellationToken: token);
    }

    private static async Task CreateQueue(IChannel channel, string queueName, CancellationToken token = default)
    {
        // Creates the queue if it does not exist.
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } },
            cancellationToken: token);
    }

    private async Task<IChannel> CreateChannel(string clientName, CancellationToken token = default)
    {
        ConnectionFactory factory = new()
        {
            HostName = _options.Hostname,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            ClientProvidedName = clientName,
        };

        _connection = await factory.CreateConnectionAsync(token);
        return await _connection.CreateChannelAsync(cancellationToken: token);
    }
}