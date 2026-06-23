using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly DocumentRepository _repository;
    private readonly EmbeddingService _embeddingService;

    public SearchController(DocumentRepository repository, EmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] string query)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);

        var results = await _repository.SearchSimilarAsync(queryEmbedding);

        return Ok(results);
    }
}