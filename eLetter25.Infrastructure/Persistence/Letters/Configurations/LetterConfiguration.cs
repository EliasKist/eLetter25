using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLetter25.Infrastructure.Persistence.Letters.Configurations;

internal sealed class LetterConfiguration : IEntityTypeConfiguration<LetterDbEntity>
{
    public void Configure(EntityTypeBuilder<LetterDbEntity> builder)
    {
        builder.ToTable("Letters", "eletter25");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.OwnerId);

        builder.HasIndex(l => l.OwnerId);
    }
}


