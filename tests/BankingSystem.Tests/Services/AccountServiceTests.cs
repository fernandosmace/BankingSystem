using BankingSystem.Application.Services;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace BankingSystem.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IAccountHistoryRepository> _accountHistoryRepositoryMock;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _accountHistoryRepositoryMock = new Mock<IAccountHistoryRepository>();

        _accountService = new AccountService(_accountRepositoryMock.Object, _accountHistoryRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAccount_Should_Create_Account_When_Valid()
    {
        var name = "User Test";
        var document = "12345678900";
        _accountRepositoryMock.Setup(repo => repo.GetByDocumentAsync(document)).ReturnsAsync((Account?)null);

        var result = await _accountService.CreateAccount(name, document);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(name);
        result.Data.Document.Should().Be(document);
        _accountRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task CreateAccount_Should_Fail_When_Document_Already_Exists()
    {
        var existingAccount = new Account("Existing User", "12345678900");
        _accountRepositoryMock.Setup(repo => repo.GetByDocumentAsync(existingAccount.Document))
            .ReturnsAsync(existingAccount);

        var result = await _accountService.CreateAccount("New User", "12345678900");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Já existe uma conta associada ao documento informado!");
        _accountRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Account_When_Exists()
    {
        var account = new Account("User Test", "12345678900");
        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(account.Id)).ReturnsAsync(account);

        var result = await _accountService.GetByIdAsync(account.Id);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(account.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Fail_When_Account_Not_Found()
    {
        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account?)null);

        var result = await _accountService.GetByIdAsync(Guid.NewGuid());

        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task TransferAsync_Should_Transfer_Funds_When_Valid()
    {
        var source = new Account("Source", "11111111111");
        var destination = new Account("Destination", "22222222222");
        var amount = 200m;

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(source.Id)).ReturnsAsync(source);
        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(destination.Id)).ReturnsAsync(destination);

        var result = await _accountService.TransferAsync(source.Id, destination.Id, amount);

        result.Success.Should().BeTrue();
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(source), Times.Once);
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(destination), Times.Once);
    }

    [Fact]
    public async Task TransferAsync_Should_Fail_When_Insufficient_Funds()
    {
        var source = new Account("Source", "11111111111");
        var destination = new Account("Destination", "22222222222");

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(source.Id)).ReturnsAsync(source);
        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(destination.Id)).ReturnsAsync(destination);

        var result = await _accountService.TransferAsync(source.Id, destination.Id, 5000m);

        result.Success.Should().BeFalse();
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateAccountByDocumentAsync_Should_Deactivate_When_Valid()
    {
        var account = new Account("User", "12345678900");
        _accountRepositoryMock.Setup(repo => repo.GetByDocumentAsync(account.Document)).ReturnsAsync(account);

        var result = await _accountService.DeactivateAccountByDocumentAsync(account.Document, "Admin");

        result.Success.Should().BeTrue();
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(account), Times.Once);
        _accountHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<AccountHistory>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateAccountByDocumentAsync_Should_Fail_When_Account_Not_Found()
    {
        _accountRepositoryMock.Setup(repo => repo.GetByDocumentAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);

        var result = await _accountService.DeactivateAccountByDocumentAsync("12345678900", "Admin");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Conta não encontrada.");
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }
}
