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

    public DocumentsController(DocumentRepository repository, EmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddChunk([FromBody] AddChunkRequestDTO requestDTO)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(requestDTO.Content);
        var id = await _repository.AddChunkAsync(requestDTO.Content, embedding);

        return Ok(new
        {
            Message = "Chunk added with embedding",
            Id = id
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetChunks()
    {
        var chunks = await _repository.GetAllChunksAsync();

        return Ok(chunks);
    }
}