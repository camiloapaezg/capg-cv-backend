# CAPG API

Backend API for the CAPG application. The service exposes a REST-style HTTP API for managing CV/profile data and file assets, backed by PostgreSQL, S3-compatible object storage, RabbitMQ, and an antivirus/file-scanning service.

> **Current status:** the repository is at version `0.0.2` (`2026-10-01`). The changelog describes this as the initial migration containing the README, changelog, and domain layer.

## Table of contents

- [Overview](#overview)
- [Features](#features)
- [Technology stack](#technology-stack)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Run locally](#run-locally)
- [API](#api)
- [File upload and scanning flow](#file-upload-and-scanning-flow)
- [Database](#database)
- [Testing](#testing)
- [Docker](#docker)
- [Resilience and middleware](#resilience-and-middleware)
- [Project structure](#project-structure)
- [Security notes](#security-notes)
- [Versioning and changelog](#versioning-and-changelog)
- [License](#license)

## Overview

`capg-cv-backend` is an ASP.NET Core 10 Web API implemented with Carter endpoint modules. The application is organized around application, domain, infrastructure, and endpoint concerns rather than traditional MVC controllers.

The API manages a CV-oriented domain including:

- Users
- Personal details
- General details
- Certification and training records
- Formal education
- Publications
- Work experience
- File metadata and file content

Files follow a separate processing pipeline: uploaded content is validated and placed in quarantine object storage, a RabbitMQ message is published, and a hosted service scans the content through the configured file-scanning service. Clean files are promoted to permanent storage and their metadata is persisted in PostgreSQL.

## Features

- ASP.NET Core `net10.0` Web API.
- Carter-based endpoint modules and route grouping.
- Entity Framework Core 10 with PostgreSQL via Npgsql.
- Automatic database migration during application startup.
- FluentValidation for create/update operations.
- S3-compatible object storage using the AWS SDK for .NET.
- Separate permanent and quarantine buckets for uploaded files.
- RabbitMQ integration for asynchronous file-scanning work.
- TCP-based antivirus/file scanning using the configured scanner host and port.
- File signature validation for supported image/PDF files and rejection of executable/script content.
- Image integrity validation using ImageSharp.
- Swagger/OpenAPI in the Development environment.
- Centralized exception handling, security headers, and resilience middleware.
- Polly resilience pipeline with timeout, exponential retry, and circuit-breaker policies.
- xUnit test project with Testcontainers support.

## Technology stack

| Technology | Version | Purpose |
|---|---:|---|
| .NET / ASP.NET Core | 10.0 | API runtime |
| Carter | 10.0.0 | Endpoint modules and routing |
| Entity Framework Core | 10.0.7 | ORM and migrations |
| PostgreSQL / Npgsql EF provider | 10.0.1 | Relational persistence |
| FluentValidation | 12.1.1 | Request/entity validation |
| AWS SDK for S3 | 4.0.22.2 | S3-compatible object storage |
| RabbitMQ.Client | 7.2.1 | Message broker integration |
| Polly.Extensions | 8.6.6 | Resilience policies |
| SixLabors.ImageSharp | 3.1.12 | Image validation |
| Swashbuckle.AspNetCore | 10.1.7 | Swagger/OpenAPI |
| xUnit | 2.9.2 | Testing |
| Testcontainers | 4.11.0 | Containerized test dependencies |

The versions above are taken from the project files in this repository.

## Architecture

The application is wired through three main layers:

```text
HTTP request
    │
    ▼
Carter endpoints
    │
    ├── FluentValidation
    │
    ▼
Application
    ├── repositories
    ├── validators
    ├── file validation
    └── hosted services
    │
    ├───────────────────────┐
    ▼                       ▼
Infrastructure          Domain
    ├── PostgreSQL
    ├── S3-compatible storage
    ├── RabbitMQ
    └── file scanner
```

`Program.cs` registers Application, Infrastructure, middleware, and Carter, then maps the Carter modules. PostgreSQL is configured through `DatabaseOptions`, and migrations are applied when the application starts.

## Prerequisites

For the full application, have the following available:

- .NET 10 SDK
- PostgreSQL
- An S3-compatible object-storage service
- RabbitMQ
- The configured file-scanning/antivirus service
- Docker, when running tests that rely on Testcontainers

The application can be developed with local services or compatible containerized services such as PostgreSQL, MinIO/S3-compatible storage, RabbitMQ, and a ClamAV-compatible scanner.

## Configuration

The application reads configuration using named sections. The repository currently defines these configuration areas:

| Section | Required values | Purpose |
|---|---|---|
| `DatabaseOptions` | `ConnectionString` | PostgreSQL connection |
| `FileValidationOptions` | `MaxSizeInBytes` | Maximum accepted upload size |
| `FilesScannerOptions` | `Hostname`, `Port` | Antivirus/file-scanning service |
| `FileStorageOptions` | `Hostname`, `Port`, `AccessKey`, `SecretKey`, `BucketName`, `QuarantineBucketName` | S3-compatible storage |
| `MessageBrokerOptions` | `Hostname`, `Port`, `Username`, `Password`, `Queues.FilesScanner` | RabbitMQ and scanning queue |

A typical development configuration can be supplied through `appsettings.Development.json`, user secrets, or environment variables. Do not store real credentials in tracked configuration files.

### Environment-variable mapping

ASP.NET Core configuration maps nested JSON properties to environment variables using `__`. For example:

```text
DatabaseOptions__ConnectionString
FileValidationOptions__MaxSizeInBytes
FilesScannerOptions__Hostname
FilesScannerOptions__Port
FileStorageOptions__Hostname
FileStorageOptions__Port
FileStorageOptions__AccessKey
FileStorageOptions__SecretKey
FileStorageOptions__BucketName
FileStorageOptions__QuarantineBucketName
MessageBrokerOptions__Hostname
MessageBrokerOptions__Port
MessageBrokerOptions__Username
MessageBrokerOptions__Password
MessageBrokerOptions__Queues__FilesScanner
```

Keeping infrastructure credentials outside source control is recommended for both local development and production deployments.

## Run locally

### 1. Restore dependencies

```bash
dotnet restore capg-hv-backend.slnx
```

### 2. Configure infrastructure

Provide the required configuration sections for PostgreSQL, S3-compatible storage, RabbitMQ, file scanning, and upload validation.

For local development, the checked-in launch settings define:

- HTTP: `http://localhost:5216`
- HTTPS: `https://localhost:7285`

The Development environment launches Swagger automatically.

### 3. Build

```bash
dotnet build capg-hv-backend.slnx
```

### 4. Run the API

```bash
dotnet run --project capg-hv-backend.api/capg-hv-backend.api.csproj
```

With the HTTPS launch profile, Swagger is available at:

```text
https://localhost:7285/swagger
```

With the HTTP launch profile:

```text
http://localhost:5216/swagger
```

Swagger is enabled only when `ASPNETCORE_ENVIRONMENT` is `Development`.

## API

All application routes use the `/api/v1` prefix. The API currently uses Carter modules rather than MVC controllers.

### Users

| Method | Endpoint | Purpose | Success |
|---|---|---|---|
| `GET` | `/api/v1/users/` | List users | `200 OK` |
| `GET` | `/api/v1/users/{id}` | Get a user | `200 OK` |
| `POST` | `/api/v1/users/` | Create a user | `201 Created` |
| `PUT` | `/api/v1/users/{id}` | Update a user | `204 No Content` |
| `DELETE` | `/api/v1/users/{id}` | Delete a user | `204 No Content` |

### Personal details

```text
/api/v1/users/{userId}/personal/
/api/v1/users/{userId}/personal/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### General details

```text
/api/v1/users/{userId}/general/
/api/v1/users/{userId}/general/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### Certification and training

```text
/api/v1/users/{userId}/certificationtraining/
/api/v1/users/{userId}/certificationtraining/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### Formal education

```text
/api/v1/users/{userId}/education/
/api/v1/users/{userId}/education/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### Publications

```text
/api/v1/users/{userId}/publication/
/api/v1/users/{userId}/publication/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### Work experience

```text
/api/v1/users/{userId}/experience/
/api/v1/users/{userId}/experience/{id}
```

Supported operations:

- `GET` collection
- `GET` by ID
- `POST`
- `PUT`
- `DELETE`

### Files

```text
/api/v1/users/{userId}/files/
/api/v1/users/{userId}/files/{id}
```

| Method | Endpoint | Purpose | Success |
|---|---|---|---|
| `POST` | `/api/v1/users/{userId}/files/` | Upload a file for asynchronous scanning | `202 Accepted` |
| `GET` | `/api/v1/users/{userId}/files/{id}` | Download a stored file | `200 OK` |
| `DELETE` | `/api/v1/users/{userId}/files/{id}` | Delete file metadata and stored content | `204 No Content` |

The upload endpoint expects a multipart form upload with a `file` field and an optional `fileName` query parameter. The file is validated, stored in quarantine, and a message is published to the configured RabbitMQ queue for asynchronous scanning.

### Validation and errors

Create/update endpoints use FluentValidation. Validation failures are returned as `422 Unprocessable Entity` for the entity-based resources. Other invalid requests can return `400 Bad Request`, while unexpected server failures are reported as `500 Internal Server Error`.

The exact response schemas and request bodies should be treated as the source of truth in Swagger/OpenAPI because they are defined directly by the endpoint implementation and domain models.

## File upload and scanning flow

File handling is deliberately asynchronous:

```text
Client
  │
  │ POST multipart/form-data
  ▼
Files endpoint
  │
  ├── validate user
  ├── validate size / filename / signature
  ├── validate image integrity when applicable
  └── store file in quarantine bucket
          │
          ▼
       RabbitMQ
       scanning queue
          │
          ▼
  FilesScannerService
          │
          ▼
   Antivirus scanner
          │
      ┌───┴────┐
   CLEAN     REJECTED/ERROR
      │
      ▼
Permanent S3 bucket
      │
      ▼
File metadata in PostgreSQL
```

The current validator:

- Rejects empty uploads.
- Enforces `FileValidationOptions:MaxSizeInBytes`.
- Sanitizes the uploaded filename.
- Accepts files identified as images or PDFs by file signature.
- Rejects executable and script signatures.
- Validates that images can actually be decoded by ImageSharp.

The scanner communicates over TCP using the ClamAV `zINSTREAM` protocol. Files are transferred in 64 KiB chunks.

The hosted `FilesScannerService` consumes the configured RabbitMQ queue, retrieves the file from quarantine, runs the scanner, moves clean files into permanent storage, records metadata, and removes the quarantined object. Rejected files are not promoted to permanent storage.

## Database

Entity Framework Core is configured to use PostgreSQL through Npgsql.

The application contains DbSets for:

- `User`
- `PersonalDetails`
- `GeneralDetails`
- `CertificationTraining`
- `FormalEducation`
- `Publication`
- `WorkExperience`
- `FileMetaData`

Database migrations are applied automatically during application startup. The startup sequence therefore expects the configured PostgreSQL instance to be reachable before the application begins serving requests.

For development-time migration commands, use the API project as the startup project, for example:

```bash
dotnet ef migrations list \
  --project capg-hv-backend.api/capg-hv-backend.api.csproj
```

Create a migration with:

```bash
dotnet ef migrations add <MigrationName> \
  --project capg-hv-backend.api/capg-hv-backend.api.csproj
```

## Testing

The solution includes a dedicated xUnit test project:

```text
capg-hv-backend.tests/
```

Run all tests with:

```bash
dotnet test capg-hv-backend.slnx
```

The test project references:

- xUnit
- Microsoft.NET.Test.Sdk
- Coverlet collector
- Testcontainers

If a test requires containerized infrastructure, Docker must be available to the test process.

## Docker

The API project contains a Dockerfile based on the official .NET 10 ASP.NET runtime and SDK images and exposes ports `8080` and `8081`.

The current Dockerfile should be reviewed before using it for a clean repository-root build: its `COPY`/`WORKDIR` paths still reference a project path named `capg-hv-backend`, while the solution currently contains `capg-hv-backend.api`.

This means the checked-in Docker configuration should be considered development-oriented until those paths are aligned with the current solution structure.

## Resilience and middleware

The application installs three custom middleware components:

1. `ExceptionHandlingMiddleware` — centralizes exception handling.
2. `SecurityHeadersMiddleware` — adds HTTP security headers.
3. `ResilienceMiddleware` — applies the configured Polly resilience pipeline.

The resilience pipeline is configured with:

- 60-second timeout
- Up to 3 retry attempts
- Exponential backoff
- Circuit breaker with a 50% failure ratio
- 30-second sampling duration
- Minimum throughput of 10 requests
- 30-second break duration

These settings are registered under the Polly resilience pipeline named `resilient-pipeline`.

## Project structure

```text
capg-cv-backend/
├── capg-hv-backend.api/
│   ├── Application/
│   │   ├── FilesValidator/
│   │   ├── HostedServices/
│   │   ├── Middlewares/
│   │   ├── Repositories/
│   │   └── Validators/
│   ├── Domain/
│   │   └── Entities/
│   ├── Endpoints/
│   │   ├── Entities/
│   │   └── Internal/
│   ├── Infrastructure/
│   │   ├── FilesScanner/
│   │   ├── MessageBroker/
│   │   └── Persistence/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── capg-hv-backend.api.csproj
├── capg-hv-backend.tests/
│   ├── Resources/
│   ├── appsettings.json
│   └── capg-hv-backend.tests.csproj
├── CHANGELOG.md
├── .dockerignore
├── .gitignore
└── capg-hv-backend.slnx
```

The exact implementation details can be followed through the dependency-registration entry points:

- `Application/DependencyInjection.cs`
- `Infrastructure/DependencyInjection.cs`
- `Application/Middlewares/DependencyInjection.cs`

## Security notes

### Do not commit infrastructure credentials

The repository currently contains test configuration with credentials for PostgreSQL, S3-compatible storage, and RabbitMQ. Those values are intentionally not reproduced in this README.

If any of those credentials are real or have ever been used against a live system, rotate them and replace committed secrets with environment variables, user secrets, a secret manager, or CI/CD secrets.

### File safety

File uploads are not immediately made public or placed directly into the permanent bucket. The current implementation first validates content and stores it in quarantine, then requests an asynchronous antivirus scan before promotion to permanent storage.

This is an important part of the application's security model and should be preserved when changing the upload pipeline.

## Versioning and changelog

The project follows Semantic Versioning and the changelog format documented by [Keep a Changelog](https://keepachangelog.com/).

See [`CHANGELOG.md`](./CHANGELOG.md) for release history.

Current recorded release:

```text
0.0.1 — 2026-04-23
Initial migration. CHANGELOG, README, and domain layer.
```

## License

No license file is currently present in the repository. Unless a license is added, the repository's code should not be assumed to be available for unrestricted redistribution or modification.
