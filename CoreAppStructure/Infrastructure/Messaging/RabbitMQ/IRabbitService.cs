namespace CoreAppStructure.Infrastructure.Messaging.RabbitMQ
{
    public interface IRabbitService
    {
        void PublishHNX(string data);
        void PublishFixReceive(string data);
    }
}
