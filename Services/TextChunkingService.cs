namespace SemanticSearchApi.Services;

public class TextChunkingService
{
    public List<string> ChunkText(string text,int chunkSize = 200, int overlap =50)
    {
        var chunks = new List<string>();

        for (int i = 0; i < text.Length; i += chunkSize - overlap)
        {
            chunks.Add(text.Substring(i,Math.Min(chunkSize, text.Length - i)));
        }

        return chunks;
    }
}