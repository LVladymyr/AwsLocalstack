using System;
using System.IO;
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

        public void Run()
        {
            Configure();
            CrateBucket();
            UploadFile();
            BucketLs();
            DownloadFile();
            DeleteBucket();
        }

        private void BucketLs()
        {
            var result = s3Client.ListObjectsAsync(BucketName).GetAwaiter().GetResult();
            foreach (var s3Object in result.S3Objects)
            {
                Console.WriteLine(s3Object.Key);
            }
        }

        private void DeleteBucket()
        {
            s3Client.DeleteObjectAsync(BucketName, FileName).GetAwaiter().GetResult();
            s3Client.DeleteBucketAsync(BucketName).GetAwaiter().GetResult();
        }

        private void DownloadFile()
        {
            using (GetObjectResponse response = s3Client.GetObjectAsync(BucketName, FileName).GetAwaiter().GetResult())
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                Console.WriteLine("Object metadata, Title: {0}", response.Metadata["x-amz-meta-title"]);
                Console.WriteLine("Content type: {0}", response.Headers["Content-Type"]);
                Console.WriteLine(reader.ReadToEnd());
            }
        }

        private void UploadFile()
        {
            var putRequest = new PutObjectRequest()
            {
                BucketName = BucketName,
                Metadata =  { ["x-amz-meta-title"] = "Title" },
                FilePath = Path.GetFileName(FileName),
                ContentType = "text/plain"
            };
            s3Client.PutObjectAsync(putRequest).GetAwaiter().GetResult();
        }

        private void CrateBucket()
        {
            s3Client.PutBucketAsync(BucketName).GetAwaiter().GetResult();
        }

        
    }
}