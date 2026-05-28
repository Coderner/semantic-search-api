using System.Text;
using System.Text.Json;
using Pgvector;

namespace SemanticSearchApi.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;

    public EmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Vector> GenerateEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = "nomic-embed-text",
            prompt = text
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "http://localhost:11434/api/embeddings",
            content
        );

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