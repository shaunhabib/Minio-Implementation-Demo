using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Minio_Implementation_Demo.Models;

namespace Minio_Implementation_Demo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MinioController : ControllerBase
    {
        private readonly MinioClient _minioClient;
        private readonly MinioOptions _minioOptions;

        public MinioController(IOptions<MinioOptions> options, MinioClient minioClient)
        {
            _minioOptions = options.Value;
            _minioClient = minioClient;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var bktExArgs = new BucketExistsArgs().WithBucket(_minioOptions.BucketName);
            var bucketExists = await _minioClient.BucketExistsAsync(bktExArgs);

            if (!bucketExists)
            {
                var bktArgs = new MakeBucketArgs().WithBucket(_minioOptions.BucketName);
                await _minioClient.MakeBucketAsync(bktArgs);
            }

            using var stream = file.OpenReadStream();
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioOptions.BucketName)
                .WithObject(file.FileName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
            );

            return Ok("File uploaded successfully to MinIO");
        }
    }
}
