namespace BankingSystem.API.Models.Requests;

public record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Amount);