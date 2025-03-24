using BankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure;

public class BankingDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountHistory> AccountHistories { get; set; }

    public BankingDbContext(DbContextOptions<BankingDbContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Flunt.Notifications.Notification>();

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Document)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(a => a.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(a => a.Document).IsUnique();
            entity.HasIndex(a => a.Id);
            entity.HasIndex(a => new { a.Name, a.Document });
        });

        modelBuilder.Entity<AccountHistory>(entity =>
        {
            entity.ToTable("AccountHistories");

            entity.HasKey(h => h.Id);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(h => h.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(h => h.Document)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(h => h.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(h => h.ResponsibleUser)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(h => h.ActionDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");


            entity.HasIndex(a => a.Id);
            entity.HasIndex(a => a.AccountId);
            entity.HasIndex(a => a.Document);
        });
    }

}
