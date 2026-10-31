using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigitalSword.Domain.Entities;

namespace DigitalSword.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.AppUserId);
            builder.Ignore(w => w.Id);

            builder.OwnsOne(w => w.Balance, balance =>
            {
                balance.Property(m => m.Amount).HasColumnName("BalanceAmount").HasColumnType("decimal(18,2)");
                balance.Property(m => m.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3);
            });
        }
    }
}