namespace BankingSystem.API.Models.Responses;

public record AccountResponse(Guid Id, string Name, string Document, decimal Balance, DateTime CreatedAt, bool IsActive);