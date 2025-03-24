using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.Repositories;
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId);
    Task<Account?> GetByDocumentAsync(string document);
    Task<IEnumerable<Account>> GetAllByFilterAsync(string filterName = "", string filterDocument = "");
    Task CreateAsync(Account account);
    Task UpdateAsync(Account account);
}