using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly DocumentRepository _repository;
    private readonly EmbeddingService _embeddingService;
    private readonly TextChunkingService _textChunkingService;

    public UploadController(
        DocumentRepository repository,
        EmbeddingService embeddingService,
        TextChunkingService textChunkingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
        _textChunkingService = textChunkingService;
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

        var chunks = _textChunkingService.ChunkText(text);

        var insertedChunkIds = new List<int>();

        foreach (var chunk in chunks)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);

            var id = await _repository.AddChunkAsync(chunk,embedding);

            insertedChunkIds.Add(id);
        }

        return Ok(new
        {
            Message = "File processed successfully",
            ChunkCount = insertedChunkIds.Count,
            ChunkIds = insertedChunkIds
        });
    }
}