using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Services;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly DocumentIngestionService _documentIngestionService;

    public UploadsController(DocumentIngestionService documentIngestionService)
    {
        _documentIngestionService = documentIngestionService;
    }

    [HttpPost("text")]
    public async Task<IActionResult> UploadTextFileAsync(IFormFile file)
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