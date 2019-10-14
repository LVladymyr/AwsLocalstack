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
        
        public async Task Run()
        {
            Configure();
            await CreateTopic();
            await GetTopicList();
            await SubscribeMessage();
            await SendMessage();
            await DeleteTopic();
        }

        private async Task GetTopicList()
        {
            var topicList = await snsClient.ListTopicsAsync();
            Console.WriteLine("Topic list:");
            foreach (var topic in topicList.Topics)
            {
                Console.WriteLine(" - {0}", topic.TopicArn);
            }
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

        private async Task SubscribeMessage()
        {
            var subscribeRequest = new SubscribeRequest(topicArn, "email", "yourmail@ukr.net");
            var subscribeResponse = await snsClient.SubscribeAsync(subscribeRequest);
        }

        private async Task DeleteTopic()
        {
            await snsClient.DeleteTopicAsync(topicArn);
        }


        private async Task CreateTopic()
        {
            var result = await snsClient.CreateTopicAsync(new CreateTopicRequest("TopicName"));
            topicArn = result.TopicArn;
        }

    }
}