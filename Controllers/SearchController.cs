using Microsoft.AspNetCore.Mvc;
using Pgvector;
using SemanticSearchApi.Repositories;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly DocumentRepository _repository;

    public SearchController(DocumentRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] string query)
    {
        var randomValues = new float[1536];

        for (int i = 0; i < randomValues.Length; i++)
        {
            randomValues[i] = Random.Shared.NextSingle();
        }

        var queryEmbedding = new Vector(randomValues);

        var results =
            await _repository.SearchSimilarAsync(queryEmbedding);

        return Ok(results);
    }
}