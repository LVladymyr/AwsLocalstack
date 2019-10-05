using System;
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

        public void Run()
        {
            Configure();
            CreateQueue();
            SendMessage();
            ReceiveMessage();
            DeleteQueue();
        }

        private void DeleteQueue()
        {
            sqsClient.DeleteQueueAsync(queueUrl).GetAwaiter().GetResult();
        }

        private void ReceiveMessage()
        {
            var result = sqsClient.ReceiveMessageAsync(queueUrl).GetAwaiter().GetResult();
            foreach (var message in result.Messages)
            {
                Console.WriteLine(message.Body);
            }
        }

        private void CreateQueue()
        {
            var result = sqsClient.CreateQueueAsync("MyQueue").GetAwaiter().GetResult();
            queueUrl = result.QueueUrl;
        }

        private void SendMessage()
        {
            sqsClient.SendMessageAsync(queueUrl, "Hello there!").GetAwaiter().GetResult();
        }
    }
}