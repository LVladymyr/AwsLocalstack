using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsServices
{
    public class SnsSqs
    {
        private AmazonSQSClient  sqsClient;
        private AmazonSimpleNotificationServiceClient snsClient;

        private string queueUrl;
        private string topicArn;

        private void Configure()
        {
            var snsConfig = new AmazonSimpleNotificationServiceConfig()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4575"
            };   

            snsClient = new AmazonSimpleNotificationServiceClient(snsConfig);
        
            var sqsConfig = new AmazonSQSConfig()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4576"
            };   

            sqsClient = new AmazonSQSClient(sqsConfig);
        }

        public async Task Run()
        {
            Configure();
            await CreateTopic();
            await CreateQueue();
            await SubscribeQueueToTopic();
            await SendMessage();
            await ReceiveMessage();
            await DeleteTopic();
            await DeleteQueue();
        }

        private async Task CreateQueue()
        {
            var result = await sqsClient.CreateQueueAsync("MyQueue");
            queueUrl = result.QueueUrl;
        }

        private async Task CreateTopic()
        {
            var result = await snsClient.CreateTopicAsync(new CreateTopicRequest("TopicName"));
            topicArn = result.TopicArn;
        }
        
        private async Task SubscribeQueueToTopic()
        {
            var subscribeRequest = new SubscribeRequest(topicArn, "sqs", queueUrl);
            await snsClient.SubscribeAsync(subscribeRequest);
        }

        private async Task DeleteTopic()
        {
            await snsClient.DeleteTopicAsync(topicArn);
        }
        
        private async Task DeleteQueue()
        {
            await sqsClient.DeleteQueueAsync(queueUrl);
        }
        
        private async Task SendMessage()
        {
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = "Test Message"
            };

            await snsClient.PublishAsync(request);
        }
        
        private async Task ReceiveMessage()
        {
            var result = await sqsClient.ReceiveMessageAsync(queueUrl);
            foreach (var message in result.Messages)
            {
                Console.WriteLine(message.Body);
            }
        }
    }
}