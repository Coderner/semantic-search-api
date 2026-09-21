using System.Text;
using System.Text.Json;
using Pgvector;

namespace SemanticSearchApi.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public EmbeddingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    }

    public async Task<Vector> GenerateEmbeddingAsync(string text)
    {
        var requestBody = new{
            model = "nomic-embed-text",
            prompt = text
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/embeddings",content);

        response.EnsureSuccessStatusCode(); 
        //throws exception for failure otherwise errors continue silently

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var embeddingArray =
            document.RootElement
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

        return new Vector(embeddingArray);
    }
}