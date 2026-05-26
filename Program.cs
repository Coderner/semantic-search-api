using SemanticSearchApi.Data;
using SemanticSearchApi.Repositories;
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

NpgsqlConnection.GlobalTypeMapper.UseVector();

SqlMapper.AddTypeHandler(new VectorTypeHandler());

builder.Services.AddScoped<DocumentRepository>();


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

