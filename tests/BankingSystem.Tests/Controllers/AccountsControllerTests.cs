using BankingSystem.API.Controllers;
using BankingSystem.API.Models.Requests;
using BankingSystem.API.Models.Responses;
using BankingSystem.Application.Interfaces.Services;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankingSystem.Tests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new AccountsController(_accountServiceMock.Object);
    }

    [Fact]
    public async Task GetAccountsByFilter_ShouldReturnOk_WhenAccountsExist()
    {
        var accounts = new List<Account>
        {
            new("User1", "123456789"),
            new("User2", "987654321")
        };

        _accountServiceMock
            .Setup(s => s.GetByFilterAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<Account>>.Ok(accounts));

        var result = await _controller.GetAccountsByFilter("User", "123");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Result<IEnumerable<AccountResponse>>>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task GetAccountsByFilter_ShouldReturnNotFound_WhenNoAccountsFound()
    {
        _accountServiceMock
            .Setup(s => s.GetByFilterAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<Account>>.Fail("No accounts found"));

        var result = await _controller.GetAccountsByFilter("NonExistent", "000");
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<Result<IEnumerable<AccountResponse>>>(notFoundResult.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnOk_WhenAccountIsCreated()
    {
        var request = new CreateAccountRequest("John Doe", "123456789");
        var account = new Account(request.Name, request.Document);

        _accountServiceMock
            .Setup(s => s.CreateAccount(request.Name, request.Document))
            .ReturnsAsync(Result<Account>.Ok(account));

        var result = await _controller.CreateAccount(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Result<AccountResponse>>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenAccountAlreadyExists()
    {
        var request = new CreateAccountRequest("John Doe", "123456789");

        _accountServiceMock
            .Setup(s => s.CreateAccount(request.Name, request.Document))
            .ReturnsAsync(Result<Account>.Fail("Já existe uma conta associada ao documento informado!"));

        var result = await _controller.CreateAccount(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<Result<AccountResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task Transfer_ShouldReturnOk_WhenTransferSucceeds()
    {
        var request = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 500);

        _accountServiceMock
            .Setup(s => s.TransferAsync(request.SourceAccountId, request.DestinationAccountId, request.Amount))
            .ReturnsAsync(Result.Ok());

        var result = await _controller.Transfer(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Result>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Transfer_ShouldReturnBadRequest_WhenTransferFails()
    {
        var request = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 500);

        _accountServiceMock
            .Setup(s => s.TransferAsync(request.SourceAccountId, request.DestinationAccountId, request.Amount))
            .ReturnsAsync(Result.Fail("Transfer failed"));

        var result = await _controller.Transfer(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<Result>(badRequestResult.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task DeactivateAccount_ShouldReturnOk_WhenDeactivationSucceeds()
    {
        var request = new DeactivateAccountRequest("123456789", "Admin");

        _accountServiceMock
            .Setup(s => s.DeactivateAccountByDocumentAsync(request.Document, request.ResponsibleUser))
            .ReturnsAsync(Result.Ok());

        var result = await _controller.DeactivateAccount(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Result>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task DeactivateAccount_ShouldReturnBadRequest_WhenDeactivationFails()
    {
        var request = new DeactivateAccountRequest("", "Admin");

        _accountServiceMock
            .Setup(s => s.DeactivateAccountByDocumentAsync(request.Document, request.ResponsibleUser))
            .ReturnsAsync(Result.Fail("Invalid document"));

        var result = await _controller.DeactivateAccount(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<Result>(badRequestResult.Value);
        Assert.False(response.Success);
    }
}
