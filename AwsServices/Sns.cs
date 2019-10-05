using System;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace AwsServices
{
    public class Sns
    {
        private AmazonSimpleNotificationServiceClient snsClient;
        private string topicArn;

        private void Configure()
        {
            var clientConfig = new AmazonSimpleNotificationServiceConfig()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4575"
            };   

            snsClient = new AmazonSimpleNotificationServiceClient(clientConfig);
        }
        
        public void Run()
        {
            Configure();
            CreateTopic();
            GetTopicList();
            SubscribeMessage();
            SendMessage();
            DeleteTopic();
        }

        private void GetTopicList()
        {
            var topicList = snsClient.ListTopicsAsync().GetAwaiter().GetResult();
            Console.WriteLine("Topic list:");
            foreach (var topic in topicList.Topics)
            {
                Console.WriteLine(" - {0}", topic.TopicArn);
            }
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

        private void SubscribeMessage()
        {
            var subscribeRequest = new SubscribeRequest(topicArn, "email", "yourmail@ukr.net");
            var subscribeResponse = snsClient.SubscribeAsync(subscribeRequest).GetAwaiter().GetResult();
        }

        private void DeleteTopic()
        {
            snsClient.DeleteTopicAsync(topicArn).GetAwaiter().GetResult();
        }


        public void CreateTopic()
        {
            var result = snsClient.CreateTopicAsync(new CreateTopicRequest("TopicName")).GetAwaiter().GetResult();
            topicArn = result.TopicArn;
        }

    }
}