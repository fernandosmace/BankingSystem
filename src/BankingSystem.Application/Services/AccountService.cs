using BankingSystem.Application.Interfaces.Services;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Repositories;
using Flunt.Notifications;
using Flunt.Validations;

namespace BankingSystem.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountHistoryRepository _accountHistoryRepository;

    public AccountService(IAccountRepository accountRepository, IAccountHistoryRepository accountHistoryRepository)
    {
        _accountRepository = accountRepository;
        _accountHistoryRepository = accountHistoryRepository;
    }

    public async Task<Result<Account>> CreateAccount(string name, string document)
    {
        var account = new Account(name, document);
        if (!account.IsValid)
            return Result<Account>.Fail(account.Notifications.ToList(), "Falha ao criar nova conta!");

        var existingAccount = await _accountRepository.GetByDocumentAsync(document);
        if (existingAccount != null)
            return Result<Account>.Fail("Já existe uma conta associada ao documento informado!");

        await _accountRepository.CreateAsync(account);
        return Result<Account>.Ok(account);
    }

    public async Task<Result<Account>> GetByIdAsync(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
            Result<Account>.Fail("Conta não encontrada.");

        return Result<Account>.Ok(account!);
    }

    public async Task<Result<IEnumerable<Account>>> GetByFilterAsync(string filterName = "", string filterDocument = "")
    {
        var accounts = await _accountRepository.GetAllByFilterAsync(filterName, filterDocument);
        return Result<IEnumerable<Account>>.Ok(accounts);
    }

    public async Task<Result> TransferAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount)
    {
        var sourceAccount = await _accountRepository.GetByIdAsync(sourceAccountId);
        if (sourceAccount == null)
            return Result.Fail("Conta de origem não encontrada.");

        var destinationAccount = await _accountRepository.GetByIdAsync(destinationAccountId);
        if (destinationAccount == null)
            return Result.Fail("Conta de destino não encontrada.");

        var transfer = sourceAccount.Transfer(amount, destinationAccount);

        if (transfer.Success)
        {
            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(destinationAccount);
        }

        return transfer;
    }

    public async Task<Result> DeactivateAccountByDocumentAsync(string document, string responsibleUser)
    {
        var deactivateAccount = new Contract<Notification>()
                .Requires()
                .IsNotNullOrWhiteSpace(document, "Document", "O Documento da conta deve seer preenchido.")
                .IsNotNullOrWhiteSpace(responsibleUser, "ResponsibleUser", "O Responsável pela desativação da conta deve ser preenchido.");

        if (!deactivateAccount.IsValid)
            return Result.Fail(deactivateAccount.Notifications.ToList());

        var account = await _accountRepository.GetByDocumentAsync(document);

        if (account == null)
            return Result.Fail("Conta não encontrada.");

        account.Deactivate();
        await _accountRepository.UpdateAsync(account);

        var accountHistory = new AccountHistory(account.Id, account.Document, "Desativação", responsibleUser);
        if (accountHistory.IsValid)
            await _accountHistoryRepository.CreateAsync(accountHistory);

        return Result.Ok("Conta desativada com sucesso.");
    }
}
