using Core.Interfaces;

namespace Test.Helpers.Fakes;

public sealed class FakeMessagingService : IMessagingService
{
 public List<(string Topic, object Message)> Published { get; } = new();

 public Task PublishAsync<T>(string topic, T message)
 {
 Published.Add((topic, message!));
 return Task.CompletedTask;
 }
}
