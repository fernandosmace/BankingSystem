using Flunt.Notifications;
using Flunt.Validations;

namespace BankingSystem.Domain.Entities
{
    public class AccountHistory : Entity
    {
        public Guid AccountId { get; private set; }
        public string Document { get; private set; }
        public DateTime ActionDate { get; private set; }
        public string Action { get; private set; }
        public string ResponsibleUser { get; private set; }

        public AccountHistory(Guid accountId, string document, string action, string responsibleUser)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Document = document;
            ActionDate = DateTime.UtcNow;
            Action = action;
            ResponsibleUser = responsibleUser;

            Validate();
        }

        public void Validate()
        {
            AddNotifications(new Contract<Notification>()
                .Requires()
                .IsNotNullOrWhiteSpace(Document, "AccountHistory.Document", "O documento não pode ser vazio")
                .IsNotNullOrWhiteSpace(Action, "AccountHistory.Action", "A ação não pode ser vazia")
                .IsNotNullOrWhiteSpace(ResponsibleUser, "AccountHistory.ResponsibleUser", "O usuário responsável não pode ser vazio")
            );
        }
    }
}
