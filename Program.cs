using SemanticSearchApi.Data;
using SemanticSearchApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Helps ASP.NET discover API endpoints.
builder.Services.AddEndpointsApiExplorer();
// Generates Swagger/OpenAPI documentation.
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddControllers();

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

