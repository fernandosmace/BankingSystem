using BankingSystem.Domain.Entities;
using FluentAssertions;

namespace BankingSystem.Tests.Entities
{
    public class AccountTests
    {
        [Fact]
        public void Account_Should_Create_Valid_Account()
        {
            // Arrange
            var account = new Account("Valid Name", "123456789");

            // Act & Assert
            account.IsValid.Should().BeTrue("A conta deve ser válida");
            account.Balance.Should().Be(1000m, "A conta deve iniciar com saldo de 1000");
            account.IsActive.Should().BeTrue("A conta deve estar ativa ao ser criada");
        }

        [Theory]
        [InlineData(null, "123456789")]
        [InlineData("", "123456789")]
        [InlineData(" ", "123456789")]
        [InlineData("Valid Name", null)]
        [InlineData("Valid Name", "")]
        [InlineData("Valid Name", " ")]
        public void Account_Should_Fail_When_Name_Or_Document_Is_Invalid(string name, string document)
        {
            // Arrange & Act
            var account = new Account(name, document);

            // Assert
            account.IsValid.Should().BeFalse("A conta não deve ser válida com nome ou documento inválido");
            account.Notifications.Should().NotBeEmpty();
        }

        [Fact]
        public void Account_Should_Deactivate_Correctly()
        {
            // Arrange
            var account = new Account("Valid Name", "123456789");

            // Act
            account.Deactivate();

            // Assert
            account.IsActive.Should().BeFalse("A conta deve ser desativada corretamente");
        }

        [Fact]
        public void Account_Should_Update_Balance()
        {
            // Arrange
            var account = new Account("Valid Name", "123456789");

            // Act
            account.UpdateBalance(500m);

            // Assert
            account.Balance.Should().Be(1500m, "O saldo deve ser atualizado corretamente");
        }

        [Fact]
        public void Account_Should_Not_Update_Balance_When_Amount_Is_Zero()
        {
            // Arrange
            var account = new Account("Valid Name", "123456789");
            var initialBalance = account.Balance;

            // Act
            account.UpdateBalance(0m);

            // Assert
            account.Balance.Should().Be(initialBalance, "O saldo não deve ser alterado quando o valor for zero");
        }

        [Fact]
        public void Transfer_Should_Succeed_When_Valid()
        {
            // Arrange
            var sourceAccount = new Account("Source Account", "123456789");
            var destinationAccount = new Account("Destination Account", "987654321");

            // Act
            var result = sourceAccount.Transfer(500m, destinationAccount);

            // Assert
            result.Success.Should().BeTrue("A transferência deve ser bem-sucedida");
            sourceAccount.Balance.Should().Be(500m, "O saldo da conta de origem deve ser reduzido");
            destinationAccount.Balance.Should().Be(1500m, "O saldo da conta de destino deve ser aumentado");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Transfer_Should_Fail_When_Amount_Is_Invalid(decimal amount)
        {
            // Arrange
            var sourceAccount = new Account("Source Account", "123456789");
            var destinationAccount = new Account("Destination Account", "987654321");

            // Act
            var result = sourceAccount.Transfer(amount, destinationAccount);

            // Assert
            result.Success.Should().BeFalse("A transferência não deve ser permitida com valores inválidos");
            result.Errors.Should().Contain(x => x.Message == "O valor de transferência deve ser maior que zero.");
        }

        [Fact]
        public void Transfer_Should_Fail_When_Insufficient_Balance()
        {
            // Arrange
            var sourceAccount = new Account("Source Account", "123456789");
            var destinationAccount = new Account("Destination Account", "987654321");

            // Act
            var result = sourceAccount.Transfer(5000m, destinationAccount);

            // Assert
            result.Success.Should().BeFalse("A transferência não deve ser permitida quando não há saldo suficiente");
            result.Errors.Should().Contain(x => x.Message == "A conta de origem não possui saldo suficiente para a transferência.");
        }

        [Fact]
        public void Transfer_Should_Fail_When_Source_Account_Is_Inactive()
        {
            // Arrange
            var sourceAccount = new Account("Source Account", "123456789");
            var destinationAccount = new Account("Destination Account", "987654321");

            sourceAccount.Deactivate();

            // Act
            var result = sourceAccount.Transfer(500m, destinationAccount);

            // Assert
            result.Success.Should().BeFalse("A transferência não deve ser permitida se a conta de origem estiver inativa");
            result.Errors.Should().Contain(x => x.Message == "A conta de origem precisa estar ativa.");
        }

        [Fact]
        public void Transfer_Should_Fail_When_Destination_Account_Is_Inactive()
        {
            // Arrange
            var sourceAccount = new Account("Source Account", "123456789");
            var destinationAccount = new Account("Destination Account", "987654321");

            destinationAccount.Deactivate();

            // Act
            var result = sourceAccount.Transfer(500m, destinationAccount);

            // Assert
            result.Success.Should().BeFalse("A transferência não deve ser permitida se a conta de destino estiver inativa");
            result.Errors.Should().Contain(x => x.Message == "A conta de destino precisa estar ativa.");
        }
    }
}