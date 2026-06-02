using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Services;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly DocumentIngestionService _documentIngestionService;

    public UploadController(DocumentIngestionService documentIngestionService)
    {
        _documentIngestionService = documentIngestionService;
    }

    [HttpPost("txt")]
    public async Task<IActionResult> UploadTxtFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        string text;

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            text = await reader.ReadToEndAsync();
        }

        var insertedChunkIds = await _documentIngestionService.IngestAsync(text);

        return Ok(new
        {
            Message = "File processed successfully",
            ChunkCount = insertedChunkIds.Count,
            ChunkIds = insertedChunkIds
        });
    }
}