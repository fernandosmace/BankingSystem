using Flunt.Notifications;

namespace BankingSystem.Domain.Entities;
public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; protected set; }
}
