namespace SolidMReader.Services.Interfaces;

public interface IAccountRepository
{
    bool AccountExists(int readingAccountId);
}