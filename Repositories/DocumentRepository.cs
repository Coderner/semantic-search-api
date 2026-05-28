using Dapper;
using SemanticSearchApi.Data;
using SemanticSearchApi.Models;
using Pgvector;

namespace SemanticSearchApi.Repositories;

public class DocumentRepository
{
    private readonly DbConnectionFactory _dbFactory;

    public DocumentRepository(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<int> AddChunkAsync(string content, Vector embedding)
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = @"
            INSERT INTO document_chunks (content, embedding)
            VALUES (@Content, @Embedding)
            RETURNING id;
        ";

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { Content = content, Embedding = embedding }
        );
    }

    public async Task<IEnumerable<DocumentChunk>> GetAllChunksAsync()
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = "SELECT * FROM document_chunks";

        return await connection.QueryAsync<DocumentChunk>(sql);
    }

    public async Task<IEnumerable<DocumentChunk>> SearchSimilarAsync(Vector queryEmbedding)
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = @"
            SELECT *
            FROM document_chunks
            ORDER BY embedding <=> @QueryEmbedding
            LIMIT 5;
        ";

        return await connection.QueryAsync<DocumentChunk>(
            sql,
            new
            {
                QueryEmbedding = queryEmbedding
            }
        );
    }
}