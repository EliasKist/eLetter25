using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLetter25.Infrastructure.Persistence.Letters;

[Table("Letters")]
public sealed class LetterDbEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset SentDate { get; set; }

    [Required]
    public DateTimeOffset CreatedDate { get; set; }

    public Guid? SenderReferenceId { get; set; }
    public Guid? RecipientReferenceId { get; set; }

    [Required]
    [MaxLength(200)]
    public string SenderName { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string SenderStreet { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string SenderPostalCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string SenderCity { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string SenderCountry { get; set; } = null!;

    [MaxLength(320)]
    public string? SenderEmail { get; set; }

    [MaxLength(50)]
    public string? SenderPhone { get; set; }

    [Required]
    [MaxLength(200)]
    public string RecipientName { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string RecipientStreet { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string RecipientPostalCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string RecipientCity { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string RecipientCountry { get; set; } = null!;

    [MaxLength(320)]
    public string? RecipientEmail { get; set; }

    [MaxLength(50)]
    public string? RecipientPhone { get; set; }

    public ICollection<LetterTagDbEntity> Tags { get; set; } = [];
}