using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqGetDemo
{
    public class RabbitConsumer : BackgroundService
    {
        public RabbitConsumer()
        {
            InitializeRabbitConsumer();
        }
        IModel Channel;
        private void InitializeRabbitConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = factory.CreateConnection();
            Channel = connection.CreateModel();
            Channel.QueueDeclare(
                queue: "rabbitForApi",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                System.Diagnostics.Debug.WriteLine(content);
                Channel.BasicAck(ea.DeliveryTag, false);
            };

            Channel.BasicConsume("rabbitForApi", false, consumer);

            return Task.CompletedTask;
        }
    }
}
