using System;
using System.Threading.Tasks;
using Amazon.SQS;

namespace AwsServices
{
    public class Sqs
    {
        private AmazonSQSClient  sqsClient;
        private string queueUrl;

        private void Configure()
        {
            var clientConfig = new AmazonSQSConfig()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4576"
            };   

            sqsClient = new AmazonSQSClient(clientConfig);
        }

        public async Task Run()
        {
            Configure();
            await CreateQueue();
            await SendMessage();
            await ReceiveMessage();
            await DeleteQueue();
        }

        private async Task DeleteQueue()
        {
            await sqsClient.DeleteQueueAsync(queueUrl);
        }

        private async Task  ReceiveMessage()
        {
            var result = await sqsClient.ReceiveMessageAsync(queueUrl);
            foreach (var message in result.Messages)
            {
                Console.WriteLine(message.Body);
            }
        }

        private async Task CreateQueue()
        {
            var result = await sqsClient.CreateQueueAsync("MyQueue");
            queueUrl = result.QueueUrl;
        }

        private async Task SendMessage()
        {
            await sqsClient.SendMessageAsync(queueUrl, "Hello there!");
        }
    }
}