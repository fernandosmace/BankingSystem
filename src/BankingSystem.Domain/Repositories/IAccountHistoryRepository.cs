using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.Repositories;
public interface IAccountHistoryRepository
{
    Task CreateAsync(AccountHistory accountHistory);
}