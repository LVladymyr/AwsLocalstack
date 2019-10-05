using System;
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

        public void Run()
        {
            Configure();
            CreateTopic();
            CreateQueue();
            SubscribeQueueToTopic();
            SendMessage();
            ReceiveMessage();
            DeleteTopic();
            DeleteQueue();
        }

        private void CreateQueue()
        {
            var result = sqsClient.CreateQueueAsync("MyQueue").GetAwaiter().GetResult();
            queueUrl = result.QueueUrl;
        }

        private void CreateTopic()
        {
            var result = snsClient.CreateTopicAsync(new CreateTopicRequest("TopicName")).GetAwaiter().GetResult();
            topicArn = result.TopicArn;
        }
        
        private void SubscribeQueueToTopic()
        {
            var subscribeRequest = new SubscribeRequest(topicArn, "sqs", queueUrl);
            snsClient.SubscribeAsync(subscribeRequest).GetAwaiter().GetResult();
        }

        private void DeleteTopic()
        {
            snsClient.DeleteTopicAsync(topicArn).GetAwaiter().GetResult();
        }
        
        private void DeleteQueue()
        {
            sqsClient.DeleteQueueAsync(queueUrl).GetAwaiter().GetResult();
        }
        
        private void SendMessage()
        {
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = "Test Message"
            };

            snsClient.PublishAsync(request).GetAwaiter().GetResult();
        }
        
        private void ReceiveMessage()
        {
            var result = sqsClient.ReceiveMessageAsync(queueUrl).GetAwaiter().GetResult();
            foreach (var message in result.Messages)
            {
                Console.WriteLine(message.Body);
            }
        }
    }
}