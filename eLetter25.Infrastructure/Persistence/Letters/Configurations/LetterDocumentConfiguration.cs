using eLetter25.Domain.Letters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLetter25.Infrastructure.Persistence.Letters.Configurations;

internal sealed class LetterDocumentConfiguration : IEntityTypeConfiguration<LetterDocumentDbEntity>
{
    public void Configure(EntityTypeBuilder<LetterDocumentDbEntity> builder)
    {
        builder.ToTable("LetterDocuments", "eletter25");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentFormat)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Store status as a string so the column is human-readable in the database.
        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.ContentHash)
            .HasMaxLength(64);

        builder.HasOne(d => d.Letter)
            .WithMany(l => l.Documents)
            .HasForeignKey(d => d.LetterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.LetterId);
    }
}

