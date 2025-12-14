using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLetter25.Infrastructure.Persistence.Letters;

[Table("LetterTags")]
public sealed class LetterTagDbEntity
{
    [Key]
    public int Id { get; set; }

    public Guid LetterId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Tag { get; set; } = null!;

    public LetterDbEntity Letter { get; set; } = null!;
}