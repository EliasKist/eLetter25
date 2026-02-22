using System.ComponentModel.DataAnnotations;

namespace eLetter25.Application.Common.Options;

public sealed class DocumentStorageOptions
{
    public const string SectionName = "DocumentStorage";

    [Required(AllowEmptyStrings = false)]
    public string BasePath { get; init; } = string.Empty;
}

