using Dapper;
using SemanticSearchApi.Data;
using SemanticSearchApi.Models;

namespace SemanticSearchApi.Repositories;

public class DocumentRepository
{
    private readonly DbConnectionFactory _dbFactory;

    public DocumentRepository(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<int> AddChunkAsync(string content)
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = @"
            INSERT INTO document_chunks (content)
            VALUES (@Content)
            RETURNING id;
        ";

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { Content = content }
        );
    }

    public async Task<IEnumerable<DocumentChunk>> GetAllChunksAsync()
    {
        using var connection = _dbFactory.CreateConnection();

        var sql = "SELECT * FROM document_chunks";

        return await connection.QueryAsync<DocumentChunk>(sql);
    }
}