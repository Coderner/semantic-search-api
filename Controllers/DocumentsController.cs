using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Models;
using SemanticSearchApi.Repositories;
using Pgvector;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentRepository _repository;

    public DocumentsController(DocumentRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> AddChunk([FromBody] AddChunkRequestDTO requestDTO)
    {
        var randomValues = new float[1536];

        for (int i = 0; i < randomValues.Length; i++)
        {
            randomValues[i] = Random.Shared.NextSingle();
        }

        var embedding = new Vector(randomValues);
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