using System.Reflection;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace cuc.obs.sample
{
    public class Program
    {
        // please change this special value to your real access key
        public static string AccessKey = "ak";
        // please change this special value to your real secret key 
        public static string SecretKey = "sk";
        // cuc obs endpoint .example: http://obs-lf.cucloud.cn
        public static string Endpoint = "http://obs-gzgy2.cucloud.cn";
        // bucket name
        public static string Bucket = "newBucket";

        //create a bucket
        public static async Task CreateBucket(AmazonS3Client client)
        {
            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = Bucket,
                    UseClientRegion = true,
                };

                var response = await client.PutBucketAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"\t Successfully create bucket:{Bucket}.");
                }
                else
                {
                    Console.WriteLine($"Could not create bucket: {Bucket}.");
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error creating bucket: '${e.Message}'");
            }
        }

        //list buckets
        public static async Task ListBucket(AmazonS3Client client)
        {
            try
            {
                var request = new ListBucketsRequest();
                ListBucketsResponse response = await client.ListBucketsAsync();
                response.Buckets.ForEach(bucket => Console.WriteLine($"\t bucket: {bucket.BucketName}"));
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error list buckets. Message:'{e.Message}' getting list of objects.");
            }
        }

        //upload object
        public static async Task UploadObject(AmazonS3Client client, string bucketName)
        {
            string filePath = Directory.GetCurrentDirectory() + "/Program.cs";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Could not find {filePath}");
            }
            string keyName = Path.GetFileName(filePath);

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filePath,
                };

                var response = await client.PutObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"\t Successfully upload {request.Key} to {bucketName}");
                }
                else
                {
                    Console.WriteLine($"\t Could not upload objet to bucket: {bucketName}");
                }
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine($"Error create objet. Message:'{e.Message}'. ");
            }
        }
        
        // copy object
        public static async Task CopyObject(AmazonS3Client client, string bucketName)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                SourceKey = "/Program.cs", // 注意这里的'/'
                DestinationBucket = bucketName,
                DestinationKey = "/Program2.cs",
            };
            try
            {
                var result = await client.CopyObjectAsync(request);
                Console.WriteLine($"\t Unexpected Status Code: {result}");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        // list object from bucket
        public static async Task ListObjects(AmazonS3Client client, string bucket)
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucket,
                    MaxKeys = 5,
                };
                ListObjectsV2Response response;
                do
                {
                    response = await client.ListObjectsV2Async(request);
                    response.S3Objects.ForEach(obj => Console.WriteLine($"\t object: {obj.Key}, Size:{obj.Size}"));
                }
                while (response.IsTruncated);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error list object. Message:'{e.Message}'.");
            }
        }

        //download object from bucket
        public static async Task GetObject(AmazonS3Client client, string bucketName, string key)
        {
            string filePath = Directory.GetCurrentDirectory() + "/load/" + key + "xx";
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };
                var response = await client.GetObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    await response.WriteResponseStreamToFileAsync($"{filePath}", true, CancellationToken.None);
                    Console.WriteLine($"\t Successfully download object: {key}.");
                }
                else
                {
                    Console.WriteLine("\t Cloud not download objet ");
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error download object failed.Message:{e.Message}");
            }
        }

        // delete objet from bucket
        public static async Task DeleteObject(AmazonS3Client client, string bucketName, string key)
        {
            try
            {
                var response = await client.DeleteObjectAsync(bucketName, key);
                Console.WriteLine($"\t Successfully delete object: {key}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"\t Could not delete object: {0}", key);
                Console.WriteLine($"Error delete object. Message:'{e.Message}");
            }
        }

        // delete Bucket
        public static async Task DeleteBucket(AmazonS3Client client, string bucketName)
        {
            var request = new DeleteBucketRequest
            {
                BucketName = bucketName,
            };
            var response = await client.DeleteBucketAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"\t Successfully delete bucket: {bucketName}");
            }
            else
            {
                Console.WriteLine($"\t Successfully delete bucket: {bucketName}");
            }
        }

        public static async Task Main()
        {
            Console.WriteLine("##### cuc sdk for .net samples #####");

            AmazonS3Config conf = new AmazonS3Config
            {
                ServiceURL = Endpoint,
            };
            AmazonS3Client client = new AmazonS3Client(AccessKey, SecretKey, conf);

            Console.WriteLine($"-->: 1.create bucket ");
            await CreateBucket(client);

            Console.WriteLine($"-->: 2.list bucket ");
            await ListBucket(client);

            Console.WriteLine("-->: 3.upload object ");
            await UploadObject(client, "newbucket");

            Console.WriteLine("-->: 4.copy object ");
            await CopyObject(client, "newbucket");

            Console.WriteLine("-->: 4.list object from bucket ");
            await ListObjects(client, "newbucket");

            Console.WriteLine("-->: 5.download object from bucket (download into ./load)");
            await GetObject(client, "newbucket", "Program.cs"); // 测试完成，记得删除本地的load文件夹

            Console.WriteLine("-->: 6.delete object from bucket ");
            await DeleteObject(client, "newbucket", "Program.cs");
            await DeleteObject(client, "newbucket", "Program2.cs");

            Console.WriteLine("-->: 7.delete bucket ");
            await DeleteBucket(client, "newbucket");
        }
    }

}