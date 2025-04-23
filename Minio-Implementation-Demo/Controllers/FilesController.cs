using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]/[action]")]
public class FilesController : ControllerBase
{
    private readonly MinioService _minioService;

    public FilesController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        using (var stream = file.OpenReadStream())
        {
            await _minioService.UploadFileAsync(file.FileName, stream, file.ContentType);
        }

        return Ok(new { message = "File uploaded successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var stream = await _minioService.GetFileAsync(fileName);
        return File(stream, "application/octet-stream", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetFileUrl(string fileName)
    {
        var url = await _minioService.GetFileUrlAsync(fileName);
        return Ok(new { fileUrl = url });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        await _minioService.DeleteFileAsync(fileName);
        return Ok(new { message = "File deleted successfully" });
    }
}