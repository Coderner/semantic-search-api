using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Models;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;
using Pgvector;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentRepository _repository;
    private readonly EmbeddingService _embeddingService;
    private readonly TextChunkingService _textChunkingService;

    public DocumentsController(
        DocumentRepository repository, 
        EmbeddingService embeddingService, 
        TextChunkingService textChunkingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
        _textChunkingService = textChunkingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddChunk([FromBody] AddChunkRequestDTO requestDTO)
    {
        var chunks = _textChunkingService.ChunkText(requestDTO.Content);
        var insertedChunkIds = new List<int>();

        foreach (var chunk in chunks)
        {
             var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);
             var id = await _repository.AddChunkAsync(chunk, embedding);
             insertedChunkIds.Add(id);
        }
        
        return Ok(new
        {
            Message = "Text Chunked and Stored",
            ChunkCounts = insertedChunkIds.Count,
            ChunkIds = insertedChunkIds
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetChunks()
    {
        var chunks = await _repository.GetAllChunksAsync();

        return Ok(chunks);
    }
}