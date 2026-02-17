namespace Core.Interfaces
{
    public interface IMessagingService
    {
        Task PublishAsync<T>(string routingKey, T message);
    }
}
