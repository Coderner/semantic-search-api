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
    private readonly DocumentIngestionService _documentIngestionService;

    public DocumentsController(DocumentRepository repository, DocumentIngestionService documentIngestionService)
    {
        _repository = repository;
        _documentIngestionService = documentIngestionService;
    }

    [HttpPost]
    public async Task<IActionResult> AddChunk([FromBody] AddChunkRequestDTO requestDTO)
    {
        var insertedChunkIds = await _documentIngestionService.IngestAsync(requestDTO.Content);

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