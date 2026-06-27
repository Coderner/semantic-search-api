using SemanticSearchApi.Repositories;

namespace SemanticSearchApi.Services;

public class DocumentIngestionService
{
    private readonly TextChunkingService _textChunkingService;
    private readonly EmbeddingService _embeddingService;
    private readonly DocumentRepository _documentRepository;

    public DocumentIngestionService(
        TextChunkingService textChunkingService,
        EmbeddingService embeddingService,
        DocumentRepository documentRepository
    )
    {
        _textChunkingService = textChunkingService;
        _embeddingService = embeddingService;
        _documentRepository = documentRepository;
    }

    public async Task<List<int>> IngestAsync(string text)
    {
        var chunks = _textChunkingService.CreateTextChunks(text);
        var insertedChunkIds = new List<int>();

        foreach (var chunk in chunks)
        {
             var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);
             var id = await _documentRepository.AddChunksAsync(chunk, embedding);
             insertedChunkIds.Add(id);
        }

        return insertedChunkIds;
    }
}