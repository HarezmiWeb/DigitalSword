using DigitalSword.Domain.Common;
using DigitalSword.Domain.ValueObjects;

namespace DigitalSword.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public MoneyControl Balance { get; private set; } = MoneyControl.Zero();
        public required string AppUserId { get; set; }

        public Wallet() { }
        public Wallet(string appUserId) { AppUserId = appUserId; Balance = MoneyControl.Zero(); }

        public void Deposit(MoneyControl amount) => Balance = Balance + amount;
        public void Withdraw(MoneyControl amount) => Balance = Balance - amount;
    }
}
