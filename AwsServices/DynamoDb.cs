using System;
using System.Collections.Generic;
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

        public void Run()
        {
            Configure();
            CreateTable();
            Write();
            Read();
            DropTable();
        }

        private void DropTable()
        {
            _dynamoClient.DeleteTableAsync(TableName).Wait();
        }

        private void CreateTable()
        {
            var request = new CreateTableRequest
            {
                TableName = TableName,
                KeySchema = new List<KeySchemaElement> { new KeySchemaElement("Id", KeyType.HASH), },
                AttributeDefinitions = new List<AttributeDefinition> { new AttributeDefinition("Id", ScalarAttributeType.N), },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5,
                }
            };
            
            _dynamoClient.CreateTableAsync(request).Wait();
        }

        private void Read()
        {
            var request = new GetItemRequest()
            {
                TableName = TableName,
                Key = { { "Id", new AttributeValue() { N = "42"} } }
            };
            _dynamoClient.GetItemAsync(request).Wait();
        }

        private void Write()
        {
            var putItemRequest = new PutItemRequest()
            {
                TableName = TableName,
                Item = 
                {
                    {
                        "Id", 
                        new AttributeValue() { N = "42"}
                    }
                }
            };
            
            _dynamoClient.PutItemAsync(putItemRequest).Wait();
        }
    }
}