using System.Text;
using System.Text.Json;

namespace SemanticSearchApi.Services;
public class OllamaLLMService
{
    private readonly HttpClient _httpClient;

    public OllamaLLMService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

        var response = await _httpClient.PostAsync("http://localhost:11434/api/generate",content);

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
