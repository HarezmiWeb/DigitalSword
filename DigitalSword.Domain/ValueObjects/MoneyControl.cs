namespace DigitalSword.Domain.ValueObjects
{
    public sealed record MoneyControl
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        private MoneyControl(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static MoneyControl Zero() => new(0, "TRY");
        public static MoneyControl TRY(decimal amount) => new(amount, "TRY");

        public static MoneyControl Create(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentException("Bakiye eksi olamaz!");
            if (string.IsNullOrEmpty(currency) || currency.Length != 3)
                throw new ArgumentException("Para birimi 3 haneli ISO kodunda olmalıdır (Örn: TRY).");
            return new MoneyControl(amount, currency);
        }

        public static MoneyControl operator +(MoneyControl left, MoneyControl right)
        {
            CheckCurrencyMatch(left, right);
            return new MoneyControl(left.Amount + right.Amount, left.Currency);
        }

        public static MoneyControl operator -(MoneyControl left, MoneyControl right)
        {
            CheckCurrencyMatch(left, right);
            if (left.Amount - right.Amount < 0)
                throw new InvalidOperationException("Yetersiz bakiye!");
            return new MoneyControl(left.Amount - right.Amount, left.Currency);
        }

        private static void CheckCurrencyMatch(MoneyControl left, MoneyControl right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Farklı para birimleri ile işlem yapılamaz!");
        }
    }
}