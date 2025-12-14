using eLetter25.Domain.Letters;

namespace eLetter25.Infrastructure.Persistence.Letters.Mappings;

public interface ILetterDomainToDbMapper
{
    LetterDbEntity MapToDbEntity(Letter letter);
}
