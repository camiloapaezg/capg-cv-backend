namespace capg_hv_backend.Infrastructure.MessageBroker;

public sealed class MessageBrokerOptions
{
    public string Hostname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Port { get; set; }

    public string Username { get; set; } = null!;

    public MessageBrokerQueues Queues { get; set; } = null!;
}