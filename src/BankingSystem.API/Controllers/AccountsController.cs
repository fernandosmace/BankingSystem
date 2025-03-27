namespace BankingSystem.API.Controllers;

using BankingSystem.API.Models.Requests;
using BankingSystem.API.Models.Responses;
using BankingSystem.Application.Interfaces.Services;
using BankingSystem.Domain.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Obtém uma lista de contas com base no filtro informado de Nome e Documento
    /// </summary>
    /// <param name="Nome">Nome à ser filtrado.</param>
    /// <param name="Documento">Documento à ser filtrado.</param>
    /// <returns>Retorna uma lista de contas com base no filtrado informado de Nome e Documento</returns>
    [HttpGet("filter")]
    [ProducesResponseType(typeof(Result<AccountResponse>), 200)]
    [ProducesResponseType(typeof(Result<AccountResponse>), 404)]
    [ProducesResponseType(typeof(Result<AccountResponse>), 500)]
    public async Task<IActionResult> GetAccountsByFilter([FromQuery] string? Nome, [FromQuery] string? Documento)
    {
        var result = await _accountService.GetByFilterAsync(Nome ?? "", Documento ?? "");

        if (result.Success)
            return Ok(Result<IEnumerable<AccountResponse>>.Ok(result.Data!.Select(account => new AccountResponse(account.Id, account.Name, account.Document, account.Balance, account.CreatedAt, account.IsActive))));

        return NotFound(Result<IEnumerable<AccountResponse>>.Fail(result.Message));
    }

    /// <summary>
    /// Registra uma nova conta
    /// </summary>
    /// <param name="name">Nome do cliente.</param>
    /// <param name="document">Documento do cliente.</param>
    /// <returns>Retorna os dados da conta cadastrada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<AccountResponse>), 201)]
    [ProducesResponseType(typeof(Result<AccountResponse>), 400)]
    [ProducesResponseType(typeof(Result<AccountResponse>), 500)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await _accountService.CreateAccount(request.Name, request.Document);

        if (result.Success)
            return Ok(Result<AccountResponse>.Ok(
                new AccountResponse(result.Data!.Id, result.Data!.Name, result.Data!.Document, result.Data!.Balance, result.Data!.CreatedAt, result.Data!.IsValid)
            ));

        return BadRequest(Result<AccountResponse>.Fail(result.Message));
    }

    /// <summary>
    /// Realiza a transferência de valores de uma conta para outra
    /// </summary>
    /// <param name="sourceAccountId">Id da conta de origem da transferência.</param>
    /// <param name="destinationAccountId">Id da conta de destino da transferência.</param>
    /// <param name="amount">Valor da transferência</param>
    /// <returns>Retorna resultado da transferência.</returns>
    [HttpPost("{id}/transfer")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result), statusCode: 400)]
    [ProducesResponseType(typeof(Result), 500)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var result = await _accountService.TransferAsync(request.SourceAccountId, request.DestinationAccountId, request.Amount);

        if (result.Success)
            return Ok(Result.Ok());

        return BadRequest(Result.Fail(result.Errors, result.Message));
    }

    /// <summary>
    /// Realiza a desativação de uma conta e salvando histórico da ação
    /// </summary>
    /// <param name="document">Documento referente a conta à ser desativada.</param>
    /// <param name="responsibleUser">Nome do responsável pela desativação da conta.</param>
    /// <returns>Retorna resultado da desativação.</returns>
    [HttpPatch("deactivate")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    [ProducesResponseType(typeof(Result), 500)]
    public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountRequest request)
    {
        var result = await _accountService.DeactivateAccountByDocumentAsync(request.Document, request.ResponsibleUser);

        if (result.Success)
            return Ok(Result.Ok());

        return BadRequest(Result.Fail(result.Errors, result.Message));
    }
}
