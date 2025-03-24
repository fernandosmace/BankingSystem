using BankingSystem.Domain.Common;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Interfaces.Services;
public interface IAccountService
{
    Task<Result<Account>> CreateAccount(string name, string document);
    Task<Result<Account>> GetByIdAsync(Guid accountId);
    Task<Result<IEnumerable<Account>>> GetByFilterAsync(string filterName = "", string filterDocument = "");
    Task<Result> TransferAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount);
    Task<Result> DeactivateAccountByDocumentAsync(string document, string responsibleUser);
}