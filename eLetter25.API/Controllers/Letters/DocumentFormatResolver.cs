using eLetter25.Domain.Letters;

namespace eLetter25.API.Controllers.Letters;

/// <summary>
/// Maps HTTP content-type strings to their corresponding <see cref="DocumentFormat"/> values.
/// Centralises the supported-format definition so both upload controllers stay in sync.
/// </summary>
internal static class DocumentFormatResolver
{
    private static readonly Dictionary<string, DocumentFormat> ContentTypeMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = DocumentFormat.Pdf,
            ["image/png"]       = DocumentFormat.Png,
            ["image/jpeg"]      = DocumentFormat.Jpeg
        };

    /// <summary>
    /// Attempts to resolve a <see cref="DocumentFormat"/> for the given content-type header value.
    /// </summary>
    /// <returns>
    /// <c>true</c> and a valid <paramref name="format"/> when the content type is supported;
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool TryResolve(string contentType, out DocumentFormat format)
        => ContentTypeMap.TryGetValue(contentType, out format);

    /// <summary>Human-readable list of accepted MIME types for use in error messages.</summary>
    public static string AcceptedTypes => "PDF, PNG, JPEG";
}

