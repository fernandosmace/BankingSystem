using BankingSystem.Domain.Entities;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Tests.Repositories;

public class AccountHistoryRepositoryTests
{
    private readonly DbContextOptions<BankingDbContext> _dbContextOptions;

    public AccountHistoryRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task CreateAsync_Should_Save_History_Successfully()
    {
        var account = new Account("History User", "12345678900");
        var history = new AccountHistory(account.Id, "12345678900", "Deposit", "Admin");

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }

        var repository = new AccountHistoryRepository(new BankingDbContext(_dbContextOptions));
        await repository.CreateAsync(history);

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            var savedHistory = await context.AccountHistories.FirstOrDefaultAsync(h => h.AccountId == account.Id);
            savedHistory.Should().NotBeNull();
            savedHistory!.Document.Should().Be("12345678900");
            savedHistory.Action.Should().Be("Deposit");
            savedHistory.ResponsibleUser.Should().Be("Admin");
        }
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_When_Error_Occurs()
    {
        await using var context = new BankingDbContext(_dbContextOptions);
        var repository = new AccountHistoryRepository(context);

        await context.DisposeAsync();

        await FluentActions
            .Invoking(() => repository.CreateAsync(new AccountHistory(Guid.NewGuid(), "12345678900", "Withdrawal", "Admin")))
            .Should().ThrowAsync<ObjectDisposedException>();
    }
}
