using BankingSystem.Domain.Common;
using Flunt.Notifications;
using Flunt.Validations;

namespace BankingSystem.Domain.Entities;
public class Account : Entity
{
    public string Name { get; private set; }
    public string Document { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Account() { }

    public Account(string name, string document)
    {
        Id = Guid.NewGuid();
        Name = name;
        Document = document;
        Balance = 1000m;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;

        Validate();
    }

    public void Validate()
    {
        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNullOrWhiteSpace(Name, "Account.Name", "O nome não pode ser vazio")
            .IsNotNullOrWhiteSpace(Document, "Account.Document", "O documento não pode ser vazio")
        );
    }

    public void Deactivate() => IsActive = false;

    public void UpdateBalance(decimal amount)
    {
        if (amount != 0)
            Balance += amount;
    }

    public Result Transfer(decimal amount, Account destinationAccount)
    {
        var transferContract = new Contract<Notification>()
                .Requires()
                .IsTrue(IsActive, "Account.IsActive", "A conta de origem precisa estar ativa.")
                .IsGreaterOrEqualsThan(Balance, amount, "Account.Balance", "A conta de origem não possui saldo suficiente para a transferência.")
                .IsGreaterThan(amount, 0, "Amount", "O valor de transferência deve ser maior que zero.")
                .IsTrue(destinationAccount.IsActive, "Account.IsActive", "A conta de destino precisa estar ativa.");

        if (transferContract.IsValid)
        {
            UpdateBalance(-amount);
            destinationAccount.UpdateBalance(amount);
            return Result.Ok("Transferência realizada com sucesso!");
        }

        return Result.Fail(transferContract.Notifications.ToList(), "Falha na realização da transferência!");
    }
}