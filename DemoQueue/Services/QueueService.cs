using DemoQueue.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DemoQueue.Services
{
    public class QueueService : IQueueService
    {
        private readonly ConnectionFactory _factory;
        private const string QUEUE_NAME = "DemoRabbitMQ";

        public QueueService()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        public Task<bool> PostMessage(MessageInputModel message)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var stringMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringMessage);

                    try
                    {
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: QUEUE_NAME,
                            basicProperties: null,
                            body: bytesMessage);
                        return Task.FromResult(true);
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(false);
                    }
                }
            }
        }
    }
}
