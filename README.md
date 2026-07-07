# RAG-based Document Q&A System
### `semantic-search-api`

A Retrieval-Augmented Generation (RAG) backend built in **ASP.NET Core 8** that ingests documents, stores them as vector embeddings in **PostgreSQL/pgvector**, and answers natural-language questions by retrieving relevant context and generating grounded answers through a locally-hosted LLM (**Ollama**) (no external API dependency, no per-token costs).

---

## What it does

Most "semantic search" demos stop at similarity search. This project implements the full RAG loop end-to-end: **chunk → embed → store → retrieve → generate**, the same pattern used in production RAG systems, built from scratch instead of wrapped around LangChain, to actually understand what is happening at each stage.

---

## Architecture

```
Ingestion:
  Raw Text/File Upload → Text Chunking Service → Embedding Service (Ollama) → pgvector storage

Query:
  User Question → Embedding Service → Cosine Similarity Search (pgvector) → Top 5 Relevant Chunks → Ollama LLM → Context-Grounded Answer
```

**Layered design:** `Controllers → Services → Repositories → Data`, with Dapper for lightweight, explicit SQL data access (no heavy ORM abstraction over vector queries).

---

## Tech Stack

| Layer              | Technology                          |
|--------------------|--------------------------------------|
| API Framework      | ASP.NET Core 8 (Web API)            |
| Data Access        | Dapper                              |
| Vector Database     | PostgreSQL + pgvector (cosine similarity) |
| Embeddings & LLM   | Ollama (local inference)            |
| Language           | C#                                  |

---

## Why pgvector

Chose PostgreSQL with the `pgvector` extension over a dedicated vector database (e.g. Pinecone, Weaviate) to keep the stack self-hosted, free, and relationally queryable and cosine similarity (`<=>` operator) handles the actual ranking.

---

## API Endpoints

| Method | Route                  | Description                                              |
|--------|-------------------------|------------------------------------------------------------|
| `POST` | `/api/documents`        | Ingest raw text then chunks, embeds, and stores it            |
| `GET`  | `/api/documents`        | Retrieve all stored chunks                                  |
| `POST` | `/api/uploads/text`     | Upload a `.txt` file for ingestion (multipart form)          |
| `POST` | `/api/search`           | Vector similarity search, returns top-5 relevant chunks   |
| `POST` | `/api/questions`        | Ask a question, retrieves context and generates an LLM answer |

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+ with the `pgvector` extension enabled
- [Ollama](https://ollama.com) running locally with an embedding + chat model pulled
