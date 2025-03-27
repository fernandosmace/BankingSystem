using BankingSystem.Domain.Entities;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Tests.Repositories;

public class AccountRepositoryTests
{
    private readonly DbContextOptions<BankingDbContext> _dbContextOptions;

    public AccountRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Account_When_Id_Exists()
    {
        var account = new Account("Test User", "12345678900");

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }

        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));
        var result = await repository.GetByIdAsync(account.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Id_Does_Not_Exist()
    {
        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));
        var result = await repository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByDocumentAsync_Should_Return_Account_When_Document_Exists()
    {
        var account = new Account("Test User", "12345678901");

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }

        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));
        var result = await repository.GetByDocumentAsync("12345678901");

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByDocumentAsync_Should_Return_Null_When_Document_Does_Not_Exist()
    {
        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));
        var result = await repository.GetByDocumentAsync("99999999999");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllByFilterAsync_Should_Return_Correct_Results()
    {
        var accounts = new List<Account>
        {
            new("Alice", "11111111111"),
            new("Bob", "22222222222"),
            new("Charlie", "33333333333"),
            new("Alice Johnson", "44444444444")
        };

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();
        }

        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));

        var result1 = await repository.GetAllByFilterAsync("Alice");
        result1.Should().HaveCount(2);

        var result2 = await repository.GetAllByFilterAsync("", "22222222222");
        result2.Should().ContainSingle();

        var result3 = await repository.GetAllByFilterAsync();
        result3.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Account_Successfully()
    {
        var account = new Account("New User", "98765432100");
        var repository = new AccountRepository(new BankingDbContext(_dbContextOptions));

        await repository.CreateAsync(account);

        using (var context = new BankingDbContext(_dbContextOptions))
        {
            var savedAccount = await context.Accounts.FindAsync(account.Id);
            savedAccount.Should().NotBeNull();
            savedAccount!.Name.Should().Be("New User");
        }
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_When_Error_Occurs()
    {
        await using var context = new BankingDbContext(_dbContextOptions);
        var repository = new AccountRepository(context);

        await context.DisposeAsync();

        await FluentActions
            .Invoking(() => repository.CreateAsync(new Account("Error User", "11111111111")))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_Exception_When_Error_Occurs()
    {
        await using var context = new BankingDbContext(_dbContextOptions);
        var repository = new AccountRepository(context);

        var account = new Account("Should Fail", "66666666666");

        await context.DisposeAsync();

        await FluentActions
            .Invoking(() => repository.UpdateAsync(account))
            .Should().ThrowAsync<ObjectDisposedException>();
    }
}
