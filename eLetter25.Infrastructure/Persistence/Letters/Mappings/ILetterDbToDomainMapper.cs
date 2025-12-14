namespace eLetter25.Infrastructure.Persistence.Letters.Mappings;

public interface ILetterDbToDomainMapper
{
    eLetter25.Domain.Letters.Letter MapToDomain(LetterDbEntity entity);
}
