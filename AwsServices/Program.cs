using System;
using System.Threading.Tasks;

namespace AwsServices
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new DynamoDb().Run();
            await new SnsSqs().Run();
            await new S3().Run();
        }
    }
}