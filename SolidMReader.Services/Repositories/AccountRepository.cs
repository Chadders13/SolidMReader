using Microsoft.Extensions.Logging;
using SolidMReader.Data.Context;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly SolidMReaderContext _dbContext;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(SolidMReaderContext dbContext, ILogger<AccountRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public bool AccountExists(int accountId)
    {
        return _dbContext.Accounts.Any(x => x.AccountId == accountId);
    }
}