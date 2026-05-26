using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Models;
using SemanticSearchApi.Repositories;

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
        var id = await _repository.AddChunkAsync(requestDTO.Content);

        return Ok(new
        {
            Message = "Chunk added",
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