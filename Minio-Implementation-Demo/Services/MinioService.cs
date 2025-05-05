using Minio;

public class MinioService
{
    #region ctor
    private readonly MinioClient _minioClient;
    private readonly string _bucketName;

    public MinioService(IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"];
        var accessKey = configuration["Minio:AccessKey"];
        var secretKey = configuration["Minio:SecretKey"];
        _bucketName = configuration["Minio:BucketName"];

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }
    #endregion

    public async Task<bool> UploadFileAsync(string objectName, Stream data, string contentType)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType);

            var res = await _minioClient.PutObjectAsync(putObjectArgs);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<Stream> GetFileAsync(string objectName)
    {
        var memoryStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<string> GetFileUrlAsync(string fileName)
    {
        try
        {
            var url = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName)
                    .WithExpiry(60 * 60) // valid for 1 hour
                );

            return url;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<bool> DeleteFileAsync(string objectName)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
        return true;
    }
}