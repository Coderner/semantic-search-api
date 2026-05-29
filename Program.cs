using SemanticSearchApi.Data;
using SemanticSearchApi.Repositories;
using SemanticSearchApi.Services;
using Npgsql;
using Pgvector.Dapper;
using Dapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Helps ASP.NET discover API endpoints.
builder.Services.AddEndpointsApiExplorer();
// Generates Swagger/OpenAPI documentation.
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DbConnectionFactory>();

#pragma warning disable CS0618 // Type or member is obsolete
NpgsqlConnection.GlobalTypeMapper.UseVector();
#pragma warning restore CS0618 // Type or member is obsolete

SqlMapper.AddTypeHandler(new VectorTypeHandler());

builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddScoped<TextChunkingService>();


var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    // Creates the OpenAPI JSON endpoint.
    app.UseSwagger();
    // Creates the browser UI.
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

