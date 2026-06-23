using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Services;

namespace SemanticSearchApi.Controllers;
public class RAGController : ControllerBase
{
    private readonly OllamaLLMService _ollamaLLMService;

    public RAGController(OllamaLLMService ollamaLLMService)
    {
        _ollamaLLMService = ollamaLLMService;
    }

    [HttpGet("test-llm")]
    public async Task<IActionResult> TestLlm()
    {
        var answer = await _ollamaLLMService.GenerateAnswerAsync(
            "What is the leave policy?",
            new List<string>
            {
                "Employees are entitled to 20 paid leaves annually."
            });

        return Ok(answer);
    }
}

