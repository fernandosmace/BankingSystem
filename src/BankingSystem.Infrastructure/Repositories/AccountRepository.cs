using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid accountId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<Account?> GetByDocumentAsync(string document)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Document == document);
    }

    public async Task<IEnumerable<Account>> GetAllByFilterAsync(string filterName = "", string filterDocument = "")
    {
        var query = _context.Accounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterName))
            query = query.Where(a => a.Name.Contains(filterName));

        if (!string.IsNullOrWhiteSpace(filterDocument))
            query = query.Where(a => a.Document == filterDocument);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task CreateAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }
}
