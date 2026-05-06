using capg_hv_backend.Endpoints.Entities;
using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace capg_hv_backend.tests.UnitTests.Infrastructure;

[Collection("Infrastructure services")]
public class MessageBrokerShould(InfrastructureFixture fixture)
{
    private readonly InfrastructureFixture _fixture = fixture;

    [Fact]
    public async Task SendAndReceiveMessage()
    {
        string queueName = "upload-queue-dev";
        FileScanRequestDto sent = new(Guid.NewGuid(), Guid.NewGuid(), "filename.png");

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
        TaskCompletionSource<bool> tcs = new();

        // Prepares message reception
        using IMessageBroker receiver = _fixture.TestHost.Services.GetRequiredService<IMessageBroker>();
        Assert.NotNull(receiver);
        await receiver.RegisterMessageHandler(queueName, async (model, args) =>
        {
            byte[] content = args.Body.ToArray();
            FileScanRequestDto? received = JsonSerializer.Deserialize<FileScanRequestDto>(content);
            Assert.NotNull(received);
            Assert.Equal(sent.Id, received.Id);
            tcs.SetResult(true);
        });

        // Sends the message
        using (IMessageBroker sender = _fixture.TestHost.Services.GetRequiredService<IMessageBroker>())
        {
            Assert.NotNull(sender);
            await sender.SendMessage(queueName, JsonSerializer.SerializeToUtf8Bytes(sent));
        }

        // Awaits for the event to be triggered
        await tcs.Task.WaitAsync(cts.Token);
    }
}