﻿namespace Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMqProducer
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        public RabbitMqProducer(RabbitMqSettings rabbitMqSettings)
        {
            _rabbitMqSettings = rabbitMqSettings;
        }

        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.HostName,
                UserName = _rabbitMqSettings.UserName,
                Password = _rabbitMqSettings.Password
            };

           /* using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _rabbitMqSettings.QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                channel.BasicPublish(exchange: "",
                                     routingKey: _rabbitMqSettings.QueueName,
                                     basicProperties: null,
                                     body: body);
            }*/
        }
    }
}