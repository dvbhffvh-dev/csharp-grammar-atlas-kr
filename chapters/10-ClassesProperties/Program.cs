if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// required는 C# 11, init은 C# 9부터 사용할 수 있습니다.
Account account = new("A-01", 100) { Owner = "Min" };
Console.WriteLine($"account: {account.Number} / {account.Owner} / {account.Balance}");
bool accepted = account.Deposit(50);
Console.WriteLine($"deposit: {accepted} / {account.Balance}");
bool rejected = account.Deposit(-10);
Console.WriteLine($"negative: {rejected} / {account.Balance}");
Console.WriteLine($"shared: {Account.Created} / {Account.Currency}");

internal sealed class Account
{
    public const string Currency = "KRW";
    private readonly string _number;
    private int _balance;

    public required string Owner { get; init; }
    public string Number => _number;
    public int Balance => _balance;
    public static int Created { get; private set; }

    public Account(string number, int openingBalance)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(openingBalance);
        _number = number;
        _balance = openingBalance;
        Created++;
    }

    public bool Deposit(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        _balance += amount;
        return true;
    }
}
