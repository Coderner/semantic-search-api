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

    public async Task<int> AddChunksAsync(string content, Vector embedding)
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

    public async Task<IEnumerable<SearchResultDTO>> SearchSimilarChunksAsync(Vector queryEmbedding)
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = @"SELECT 
                    id, 
                    content, 
                    embedding <=> @QueryEmbedding AS SimilarityScore
                    FROM document_chunks
                    ORDER BY SimilarityScore
                    LIMIT 5;";

        return await connection.QueryAsync<SearchResultDTO>(
            sql,
            new{QueryEmbedding = queryEmbedding}
        );
    }
}