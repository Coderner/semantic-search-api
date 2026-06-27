using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;
namespace SemanticSearchApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly OllamaLLMService _ollamaLLMService;
    private readonly EmbeddingService _embeddingService;
    private readonly DocumentRepository _documentRepository;

    public QuestionsController(
        OllamaLLMService ollamaLLMService, 
        EmbeddingService embeddingService, 
        DocumentRepository documentRepository
    )
    {
        _ollamaLLMService = ollamaLLMService;
        _embeddingService = embeddingService;
        _documentRepository = documentRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AskQuestionsAsync([FromBody] string question)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);

        var searchResults = await _documentRepository.SearchSimilarChunksAsync(queryEmbedding);

        var contextChunks = searchResults.Select(x => x.Content).ToList();

        var answer = await _ollamaLLMService.GenerateAnswerAsync(question, contextChunks);

        return Ok(answer);
    }
}

