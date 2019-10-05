using System;

namespace AwsServices
{
    class Program
    {
        static void Main(string[] args)
        {
            // new DynamoDb().Run();
            // new SnsSqs().Run();
            new S3().Run();
        }
    }
}