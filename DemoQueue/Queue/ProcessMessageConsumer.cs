using DemoQueue.Config;
using DemoQueue.interfaces;
using DemoQueue.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoQueue.Queue
{
    public class ProcessMessageConsumer : BackgroundService
    {
        private readonly RabbitMqConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public ProcessMessageConsumer(IOptions<RabbitMqConfiguration> option,
                                      IServiceProvider serviceProvider)
        {
            _config = option.Value;
            _serviceProvider = serviceProvider;


            var factory = new ConnectionFactory
            {
                HostName = _config.Host
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _config.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonConvert.DeserializeObject<MessageInputModel>(contentString);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_config.Queue, false, consumer);

            Console.WriteLine("AEEEEEEEEEEEEEEE Porraaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            return Task.CompletedTask;
        }

        public void NotifyUSer(MessageInputModel message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                notificationService.NotifyUser(message.FromId, message.ToId, message.Content);
            }
        }
    }
}
