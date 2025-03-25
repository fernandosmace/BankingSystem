using BankingSystem.Domain.Entities;
using FluentAssertions;

namespace BankingSystem.Tests.Entities
{
    public class AccountHistoryTests
    {
        [Fact]
        public void AccountHistory_Should_Create_Valid_AccountHistory()
        {
            // Arrange
            var accountHistory = new AccountHistory(Guid.NewGuid(), "123456789", "Deposit", "User1");

            // Act & Assert
            accountHistory.IsValid.Should().BeTrue("O histórico de conta deve ser válido");
            accountHistory.AccountId.Should().NotBeEmpty("O ID da conta não pode estar vazio");
            accountHistory.Document.Should().Be("123456789", "O documento deve ser igual ao informado");
            accountHistory.Action.Should().Be("Deposit", "A ação deve ser igual ao informado");
            accountHistory.ResponsibleUser.Should().Be("User1", "O responsável deve ser igual ao informado");
        }

        [Theory]
        [InlineData(null, "Deposit", "User1")]
        [InlineData("", "Deposit", "User1")]
        [InlineData(" ", "Deposit", "User1")]
        [InlineData("Valid Document", null, "User1")]
        [InlineData("Valid Document", "", "User1")]
        [InlineData("Valid Document", " ", "User1")]
        [InlineData("Valid Document", "Deposit", null)]
        [InlineData("Valid Document", "Deposit", "")]
        [InlineData("Valid Document", "Deposit", " ")]
        public void AccountHistory_Should_Fail_When_Any_Field_Is_Invalid(string document, string action, string responsibleUser)
        {
            // Arrange & Act
            var accountHistory = new AccountHistory(Guid.NewGuid(), document, action, responsibleUser);

            // Assert
            accountHistory.IsValid.Should().BeFalse("O histórico de conta não deve ser válido com campo inválido");
            accountHistory.Notifications.Should().NotBeEmpty("Deve haver notificações de erro quando campos são inválidos");
        }

        [Fact]
        public void AccountHistory_Should_Have_Valid_ActionDate()
        {
            // Arrange
            var accountHistory = new AccountHistory(Guid.NewGuid(), "123456789", "Deposit", "User1");

            // Act & Assert
            accountHistory.ActionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1), "A data da ação deve ser próxima ao momento da criação");
        }

        [Fact]
        public void AccountHistory_Should_Fail_When_Document_Is_Invalid()
        {
            // Arrange
            var accountHistory = new AccountHistory(Guid.NewGuid(), "", "Deposit", "User1");

            // Act & Assert
            accountHistory.IsValid.Should().BeFalse("O documento não pode ser vazio");
            accountHistory.Notifications.Should().Contain(n => n.Message == "O documento não pode ser vazio");
        }

        [Fact]
        public void AccountHistory_Should_Fail_When_Action_Is_Invalid()
        {
            // Arrange
            var accountHistory = new AccountHistory(Guid.NewGuid(), "123456789", "", "User1");

            // Act & Assert
            accountHistory.IsValid.Should().BeFalse("A ação não pode ser vazia");
            accountHistory.Notifications.Should().Contain(n => n.Message == "A ação não pode ser vazia");
        }

        [Fact]
        public void AccountHistory_Should_Fail_When_ResponsibleUser_Is_Invalid()
        {
            // Arrange
            var accountHistory = new AccountHistory(Guid.NewGuid(), "123456789", "Deposit", "");

            // Act & Assert
            accountHistory.IsValid.Should().BeFalse("O usuário responsável não pode ser vazio");
            accountHistory.Notifications.Should().Contain(n => n.Message == "O usuário responsável não pode ser vazio");
        }
    }
}
