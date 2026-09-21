using System.Text;
using System.Text.Json;

namespace SemanticSearchApi.Services;
public class OllamaLLMService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public OllamaLLMService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    }
    public async Task<string> GenerateAnswerAsync(string question, List<string> contextChunks)
    {
        var context = string.Join("\n\n", contextChunks);
        var prompt = $"""
        Answer the question using ONLY the provided context.

        Context:{context}
        Question:{question}

        If the answer is not present in the context, say:
        "I could not find the answer in the provided documents."
        """;

        var requestBody = new{
            model = "llama3.2",
            prompt = prompt,
            stream = false
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/generate",content);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

         var result =
        JsonSerializer.Deserialize<OllamaGenerateResponse>(
            json,
            new JsonSerializerOptions{
                PropertyNameCaseInsensitive = true
            }
        );

        return result?.Response ?? string.Empty;
    }
}
