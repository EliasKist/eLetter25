# API Response Format

Alle Endpunkte verwenden ein konsistentes Result-Pattern für Erfolgs- und Fehlerantworten.

## Success Response

Bei erfolgreichen Operationen wird das Ergebnis direkt zurückgegeben:

### Register Success (200 OK)
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "message": "User successfully registered"
}
```

### Login Success (200 OK)
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## Error Response

Bei Fehlern wird eine strukturierte Error-Liste zurückgegeben:

### Register Error (400 Bad Request)
```json
{
  "errors": [
    {
      "code": "Validation",
      "message": "Password requires at least one non-alphanumeric character"
    },
    {
      "code": "Validation",
      "message": "Password requires at least one uppercase letter"
    }
  ]
}
```

### Login Error (401 Unauthorized)
```json
{
  "errors": [
    {
      "code": "Unauthorized",
      "message": "Invalid email or password"
    }
  ]
}
```

---

## Error Codes

Die folgenden Error-Codes werden verwendet:

| Code | HTTP Status | Beschreibung |
|------|-------------|--------------|
| `Validation` | 400 | Eingabe-Validierungsfehler |
| `NotFound` | 404 | Ressource nicht gefunden |
| `Unauthorized` | 401 | Authentifizierung fehlgeschlagen |
| `Conflict` | 409 | Konflikt (z.B. Duplikat) |
| `Internal` | 500 | Interner Serverfehler |

---

## Result Pattern

Alle Use Cases im Application-Layer verwenden das `Result<T>` Pattern:

```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public T? Value { get; }
    public IReadOnlyList<Error> Errors { get; }
}

public sealed record Error(string Code, string Message);
```

### Vorteile

- ✅ **Konsistenz**: Alle Endpunkte folgen dem gleichen Muster
- ✅ **Typsicherheit**: Compiler-geprüfte Error-Behandlung
- ✅ **Mehrere Fehler**: Alle Validierungsfehler auf einmal
- ✅ **Strukturierte Fehler**: Code + Message für Client-seitige Verarbeitung
- ✅ **Keine Exceptions**: Fehler sind Teil des normalen Kontrollflusses

