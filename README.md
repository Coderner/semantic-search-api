# RAG-based Document Q&A System
### `Semantic Search API`

A Retrieval-Augmented Generation (RAG) backend built with **ASP.NET Core 10** that ingests documents, stores them as vector embeddings in **PostgreSQL with pgvector**, and answers natural-language questions by retrieving relevant context and generating grounded answers through a locally-hosted LLM (**Ollama**) (no external API dependency, no per-token costs, no data leaving your machine).

---

## Why This Project

Most semantic search demos stop at similarity search. This project implements the full RAG pipeline end-to-end: 

**chunk → embed → store → retrieve → generate**

The same pattern used in production RAG systems, built from scratch without wrapping LangChain or similar abstractions, to actually understand what is happening at each stage.

---

## How It Works

### Ingestion Flow
```
  Raw Text/File Upload
        ↓
  Text Chunking Service   (splits text into overlapping chunks)
        ↓
  Embedding Service       (Ollama: nomic-embed-text)
        ↓
  PostgreSQL + pgvector   (stores chunk + embedding)
```

### Query Flow
```
  User Question
       ↓
  Embedding Service       (embeds the question → vector)
       ↓
  Cosine Similarity Search (pgvector finds top 5 most similar chunks)
       ↓
  Ollama LLM              (llama3.2 generates answer from retrieved context)
       ↓
  Context-Grounded Answer
```

---

## Tech Stack

| Layer              | Technology                          |
|--------------------|--------------------------------------|
| API Framework      | ASP.NET Core 10 Web API            |
| Data Access        | Dapper  (raw SQL)                            |
| Database     | PostgreSQL + pgvector extension |
| Embeddings & LLM   | Ollama (nomic-embed-text and llama 3.2)            |
| Containerisation           | Docker + Docker Compose                                |
| API Docs           | Swagger|

---

## Project Structure
```
SemanticSearchApi/
├── Controllers/
├──Services/
|  ├──EmbeddingService
|  ├──OllamaLLMService
|  ├──TextChunkingService
|  ├──DocumentIngestionService
├──Repositories/
├──Data/
├──Models/
├──DTOs/
├──init.sql
├──Dockerfile
├──docker-compose.yml
├──.env.example
├──appsettings.json
├──Program.cs
```
---
## Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Ollama](https://ollama.com) running locally
- Pull the required models:
```bash
ollama pull llama3.2
ollama pull nomic-embed-text
```

---
## Getting Started
**1. Clone the repo**
```bash
git clone https://github.com/Coderner/semantic-search-api.git
cd semantic-search-api
```
**2. Set up environment variables**
```bash
cp .env.example .env
```
**3. Start the system**
```bash
docker-compose up --build
```

This will:
- Start a PostgreSQL 17 + pgvector container
- Run `init.sql` automatically to create the schema
- Build and start the API container

---

## Architecture Diagram
```
Your Machine
|
├──Ollama (native, port 11434)
|    ├──nomic-embed-text
|    ├──llama3.2
|
├──Docker
     ├──semantic_api (port 5050)
     |      ├──ASP.NET Core 10 API
     |          ├──host.docker.internal:11434 (Ollama)
     |          ├──postgres:5432 (internal Docker network)
     |
     ├──semantic_postgres (port 5432)
            ├──PostgreSQL 17 + pgvector
                 ├──volume: postgres_data (persisted on host)
```
---
## Key Design Decisions

- **Dapper over Entity Framework** - explicit SQL gives full control over queries, especially for pgvector cosine similarity operations that ORMs handle poorly
- **Local LLM via Ollama**- zero API cost, no data leaves the machine, works fully offline
- **pgvector for similarity search** - native Postgres extension, no separate vector database needed, cosine similarity search over 768-dimensional embeddings
- **Multi-stage Docker build** - SDK image for building, ASP.NET runtime image for running, final image is in smaller size
- **Environment-variable driven config**  - connection string and Ollama URL injected at runtime, same image runs in any environment

