using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsServices
{
    public class S3
    {
        private const string BucketName = "MyBucket";
        private const string FileName = "FileToUpload.txt";
        
        private AmazonS3Client s3Client;

        private void Configure()
        {
            var clientConfig = new AmazonS3Config()
            {
                UseHttp = true,
                ServiceURL = "http://localhost:4572"
            };

            s3Client = new AmazonS3Client(clientConfig);
        }

        public async Task Run()
        {
            Configure();
            await CrateBucket();
            await UploadFile();
            await BucketLs();
            await DownloadFile();
            await DeleteBucket();
        }

        private async Task BucketLs()
        {
            var result = await s3Client.ListObjectsAsync(BucketName);
            foreach (var s3Object in result.S3Objects)
            {
                Console.WriteLine(s3Object.Key);
            }
        }

        private async Task DeleteBucket()
        {
            await s3Client.DeleteObjectAsync(BucketName, FileName);
            await s3Client.DeleteBucketAsync(BucketName);
        }

        private async Task DownloadFile()
        {
            using (GetObjectResponse response = await s3Client.GetObjectAsync(BucketName, FileName))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                Console.WriteLine("Object metadata, Title: {0}", response.Metadata["x-amz-meta-title"]);
                Console.WriteLine("Content type: {0}", response.Headers["Content-Type"]);
                Console.WriteLine(reader.ReadToEnd());
            }
        }

        private async Task UploadFile()
        {
            var putRequest = new PutObjectRequest()
            {
                BucketName = BucketName,
                Metadata =  { ["x-amz-meta-title"] = "Title" },
                FilePath = Path.GetFileName(FileName),
                ContentType = "text/plain"
            };
            await s3Client.PutObjectAsync(putRequest);
        }

        private async Task CrateBucket()
        {
            await s3Client.PutBucketAsync(BucketName);
        }

        
    }
}