# eLetter25

Eine ASP.NET Core-Anwendung zur Verwaltung von Briefen und deren Metadaten mit JWT-basierter Authentifizierung und .NET Aspire für lokale Orchestrierung.

## 🏗️ Architektur

Das Projekt folgt **Clean Architecture** und **Domain-Driven Design (DDD)** mit vier Schichten:

- **Domain** – Geschäftslogik, Entities, Value Objects
- **Application** – Use Cases, Commands/Handlers (MediatR), Ports (Interfaces)
- **Infrastructure** – Persistenz (EF Core, SQL Server/PostgreSQL), Services (JWT, Identity), Domain-Event-Dispatch
- **API** – REST Controllers (ASP.NET Core MVC), OpenAPI/Scalar

### Architektur-Pattern

- **CQRS**: Commands/Queries via MediatR
- **Ports & Adapters**: Application definiert Interfaces, Infrastructure implementiert
- **Repository Pattern**: Datenzugriff abstrahiert
- **Unit of Work**: Transaktionsgrenzen explizit
- **Domain Events**: Events aus Domain Entities werden nach Persistenz dispatcht

## 🛠️ Technologien

- .NET 10.0
- Entity Framework Core 10 (SQL Server + PostgreSQL)
- ASP.NET Core Identity + JWT (Login/Register)
- MediatR (CQRS-Light + Domain Events)
- .NET Aspire (lokale Orchestrierung)
- Scalar/OpenAPI für API-Dokumentation

## 📦 Projekt-Struktur

```
eLetter25/
├── eLetter25.Domain/              # Entities, Value Objects, Business Rules
├── eLetter25.Application/         # Use Cases, Commands, Handlers, Ports
│   ├── Auth/                      # Authentication Use Cases
│   │   ├── Contracts/             # Request DTOs
│   │   ├── Ports/                 # Interfaces (IJwtTokenGenerator, etc.)
│   │   └── UseCases/              # RegisterUser, LoginUser
│   └── Letters/                   # Letter Management Use Cases
├── eLetter25.Infrastructure/      # EF Core, SQL Server/PostgreSQL, Services
│   ├── Auth/                      # Authentication Services & Data
│   │   ├── Data/                  # ApplicationUser, DbContext
│   │   └── Services/              # JwtTokenGenerator, UserRegistrationService
│   └── Persistence/               # Repositories, Mappings
├── eLetter25.API/                 # REST API (Controllers)
│   └── Auth/Controllers/          # RegisterController, LoginController
├── eLetter25.Host/                # .NET Aspire Orchestration
├── eLetter25.Client/              # Angular Frontend
├── eLetter25.Domain.Tests/         # Domain Unit Tests
└── eLetter25.Infrastructure.Tests/ # Infrastructure Tests
```

## 🚀 Schnellstart

### Voraussetzungen

- **.NET 10.0 SDK** installiert
- **Docker Desktop** installiert und **gestartet** (für SQL Server und PostgreSQL)

### 1. User Secrets konfigurieren

Der JWT SecretKey muss in den User Secrets des API-Projekts gespeichert werden, da `Jwt:SecretKey` beim Start validiert wird:

```powershell
# JWT Secret Key setzen (mindestens 32 Zeichen für HS256)
dotnet user-secrets set "Jwt:SecretKey" "your-super-secret-key-min-32-chars-long-for-hs256-algorithm" --project eLetter25.API
```

Die JWT Expiration Time wird in der `appsettings.json` des API-Projekts konfiguriert.

### 2. Anwendung starten

```powershell
# Aspire Host starten (startet SQL Server + PostgreSQL + Angular Client)
dotnet run --project eLetter25.Host
```

**Das war's!** Die Datenbank-Migrationen werden automatisch beim Start der API im Development-Modus ausgeführt.

- **API:** `https://localhost:7xxx` (Port wird im Terminal angezeigt)
- **Aspire Dashboard:** `http://localhost:15000`
- **Angular Client:** `http://localhost:4200`

## 📡 API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Benutzerregistrierung
- `POST /api/auth/login` - Login (liefert JWT-Token)

> Hinweis: Für Letters existiert aktuell nur der Application-Use-Case (CreateLetter) – es gibt noch keinen API-Endpoint dafür.

### Sonstiges
- `GET /` - Health/Info-Endpoint (Textantwort)

Vollständige API-Dokumentation (Development): `https://localhost:7xxx/scalar/v1` (Scalar UI)

## 📖 Dokumentation

Detaillierte Informationen zur Architektur und Entwicklung:

- [Architektur-Dokumentation](Architektur.md)
- [Coding-Guidelines](.github/copilot-instructions.md)
