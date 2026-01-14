# Auth - Authentifizierungsmodul

Dieses Verzeichnis enthält alle Komponenten, die mit der Authentifizierung und Autorisierung zu tun haben.

## Struktur

```
Auth/
├── Controllers/
│   └── AuthController.cs       # API-Endpunkte für Auth
├── Models/
│   └── AuthModels.cs          # Request/Response DTOs
└── Options/
    └── JwtOptions.cs          # JWT-Konfiguration
```

## Controllers

### AuthController
**Route:** `/api/auth`

Verwaltet Benutzerregistrierung und Authentifizierung.

**Endpoints:**
- `POST /api/auth/register` - Benutzerregistrierung
- `POST /api/auth/login` - Login mit JWT-Token-Generierung

## Models

### RegisterRequest
Request-DTO für die Benutzerregistrierung.

**Properties:**
- `Email` (string) - E-Mail-Adresse des Benutzers
- `Password` (string) - Passwort
- `EnableNotifications` (bool) - Optional, Standard: false

### LoginRequest
Request-DTO für die Benutzeranmeldung.

**Properties:**
- `Email` (string) - E-Mail-Adresse
- `Password` (string) - Passwort

### LoginResponse
Response-DTO nach erfolgreicher Anmeldung.

**Properties:**
- `AccessToken` (string) - JWT Access Token

## Options

### JwtOptions
Konfigurationsklasse für JWT-Einstellungen (Options Pattern).

**Configuration Section:** `"Jwt"`

**Properties:**
- `SecretKey` (string) - Geheimer Schlüssel für Token-Signierung
- `Issuer` (string) - Token-Aussteller
- `Audience` (string) - Token-Zielgruppe
- `ExpirationMinutes` (int) - Token-Gültigkeit in Minuten

**Beispiel appsettings.json:**
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "eLetter25.API",
    "Audience": "eLetter25.Client",
    "ExpirationMinutes": 60
  }
}
```

## Architektur-Prinzipien

✅ **Feature-basierte Organisation**: Alle Auth-relevanten Dateien an einem Ort  
✅ **Clean Architecture**: Controller → Application → Domain → Infrastructure  
✅ **Single Responsibility Principle**: Ein Controller pro Verantwortlichkeit (Register vs. Login)  
✅ **Separation of Concerns**: Models, Options und Controller getrennt  
✅ **Options Pattern**: Konfiguration via appsettings.json  
✅ **DTOs als Records**: Immutability und Value Semantics  
✅ **Minimal Dependencies**: Jeder Controller hat nur die Abhängigkeiten, die er wirklich braucht  

## Abhängigkeiten

- **Infrastructure.Auth.Data**: `ApplicationUser`, `MsIdentityDbContext`
- **Infrastructure.Persistence**: `AppDbContext` (für Transaktionen)
- **Microsoft.AspNetCore.Identity**: User Management
- **Microsoft.IdentityModel.Tokens**: JWT-Token-Generierung

