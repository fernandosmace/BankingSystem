using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Repositories;

namespace BankingSystem.Infrastructure.Repositories;

public class AccountHistoryRepository : IAccountHistoryRepository
{
    private readonly BankingDbContext _context;

    public AccountHistoryRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(AccountHistory accountHistory)
    {
        await _context.AccountHistories.AddAsync(accountHistory);
        await _context.SaveChangesAsync();
    }
}
