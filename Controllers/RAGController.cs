using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;
using System.Diagnostics;

namespace SemanticSearchApi.Controllers;
public class RAGController : ControllerBase
{
    private readonly OllamaLLMService _ollamaLLMService;
    private readonly EmbeddingService _embeddingService;
    private readonly DocumentRepository _documentRepository;

    public RAGController(
        OllamaLLMService ollamaLLMService, 
        EmbeddingService embeddingService, 
        DocumentRepository documentRepository
    )
    {
        _ollamaLLMService = ollamaLLMService;
        _embeddingService = embeddingService;
        _documentRepository = documentRepository;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] string question)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);

        var searchResults = await _documentRepository.SearchSimilarAsync(queryEmbedding);

        var contextChunks = searchResults.Select(x => x.Content).ToList();

        var answer = await _ollamaLLMService.GenerateAnswerAsync(question, contextChunks);

        return Ok(answer);
    }
}

