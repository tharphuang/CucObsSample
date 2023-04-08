# CUCObsSample
cuc obs是[联通云](https://www.cucloud.cn/product/oss.html)对外提供的一款对象存储产品，它在设计上严格遵守AWS S3协议，所以很大程度的兼容了AWS S3的各类sdk。
这里给出了使用[aws s3的.Net sdk](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/NS3.html)来连接联通云的对象存储服务的示例。
此处使用的sdk版本为
| sdk         |  version   |
| :---------- | :--------: |
| .Net        |  7.0.201   |
| AWSSDK.Core | 3.7.105.18 |
| AWSSDK.S3   | 3.7.103.26 |


# dotnet 示例项目
## 1. 创建项目
```
mkdir dotnet
cd dotnet 
```
## 2. 添加sdk
```
cd cucsample
dotnet add package Amazon.S3
dotnet add package AWSSDK.Core
```
## 3. 快速使用
连接对象存储服务
```C#
AmazonS3Config conf = new AmazonS3Config
{
    ServiceURL = Endpoint,
};
AmazonS3Client client = new AmazonS3Client(AccessKey, SecretKey, conf);
```
创建存储桶
```C# 
var request = new PutBucketRequest
{
    BucketName = Bucket,
    UseClientRegion = true,
};

var response = await client.PutBucketAsync(request);
```
列举用户空间下的存储桶
```C#
var request = new ListBucketsRequest();
ListBucketsResponse response = await client.ListBucketsAsync();
```
上传对象到存储桶中
```C#
 var request = new PutObjectRequest
{
    BucketName = bucketName,
    Key = keyName,
    FilePath = filePath,
};
var response = await client.PutObjectAsync(request);
```
列举存储桶中的对象
```C#
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
```
从存储桶中下载对象
```C#
var request = new GetObjectRequest
{
    BucketName = bucketName,
    Key = key
};
var response = await client.GetObjectAsync(request);
await response.WriteResponseStreamToFileAsync($"{filePath}", true, CancellationToken.None);
```
删除存储桶中的对象
```C#
var response = await client.DeleteObjectAsync(bucketName,key);
```
删除存储桶
```C#
var request = new DeleteBucketRequest
{
    BucketName = bucketName,
};
var response = await client.DeleteBucketAsync(request);
```
## 4. 示例代码运行结果
```
[root]_ ~/CucObsSample/dotnet>: dotnet run
##### cuc sdk for .net samples #####
-->: 1.create bucket 
         Successfully create bucket:newBucket.
-->: 2.list bucket 
         bucket: newbucket
-->: 3.upload object 
         Successfully upload Program.cs to newbucket
-->: 4.list object from bucket 
         object: Program.cs, Size:7856
-->: 5.download object from bucket
         Successfully download object: Program.cs.
-->: 6.delete object from bucket 
         Successfully delete object: Program.cs
-->: 7.delete bucket 
         Successfully delete bucket: newbucket
```
## 5. 其它
示例项目中只是给出了sdk中几个较为常用的方法，其它方法的是使用可以参考aws s3的官方文档

