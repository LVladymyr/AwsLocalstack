using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace AwsServices
{
    public class DynamoDb
    {
        private const string TableName = "Todo";
        
        private AmazonDynamoDBClient _dynamoClient;
        
        public void Configure()
        {
            var clientConfig = new AmazonDynamoDBConfig()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4569"
            };        
            
            _dynamoClient = new AmazonDynamoDBClient(clientConfig);
        }

        public async Task Run()
        {
            Configure();
            await CreateTable();
            await PutItem();
            await GetItem();
            await DropTable();
        }

        private async Task DropTable()
        {
            await _dynamoClient.DeleteTableAsync(TableName);
        }

        private async Task CreateTable()
        {
            var request = new CreateTableRequest
            {
                TableName = TableName,
                KeySchema = new List<KeySchemaElement> { new KeySchemaElement("Id", KeyType.HASH), },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition("Id", ScalarAttributeType.N),
                    new AttributeDefinition("Name", ScalarAttributeType.S)
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5,
                }
            };
            
            await _dynamoClient.CreateTableAsync(request);
        }

        private async Task GetItem()
        {
            var request = new GetItemRequest()
            {
                TableName = TableName,
                Key = { { "Id", new AttributeValue() { N = "42"} } }
            };
            var result = await _dynamoClient.GetItemAsync(request);
            Console.WriteLine(result.Item.Values);
        }

        private async Task PutItem()
        {
            var putItemRequest = new PutItemRequest()
            {
                TableName = TableName,
                Item = 
                {
                    {
                        "Id", new AttributeValue() { N = "42"}
                    },
                    {
                        "Name", new AttributeValue() {S = "Get Up Early"}
                    }
                }
            };
            await _dynamoClient.PutItemAsync(putItemRequest);
        }
    }
}