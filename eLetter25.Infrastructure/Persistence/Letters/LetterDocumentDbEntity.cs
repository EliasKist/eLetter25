using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.Enums;

namespace eLetter25.Infrastructure.Persistence.Letters;

[Table("LetterDocuments")]
public sealed class LetterDocumentDbEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid LetterId { get; set; }

    [Required]
    public DocumentFormat DocumentFormat { get; set; }

    [Required]
    public DocumentStatus Status { get; set; }

    /// <summary>SHA-256 hex digest (64 characters). Null until the file has been stored.</summary>
    [MaxLength(64)]
    public string? ContentHash { get; set; }

    public long? SizeInBytes { get; set; }

    public LetterDbEntity Letter { get; set; } = null!;
}

