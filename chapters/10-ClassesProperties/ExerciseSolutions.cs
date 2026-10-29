internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 초기화 전용 속성은 객체를 만드는 시점에 지정합니다.
        Account account = new("B-02", 20) { Owner = "Jin" };
        Console.WriteLine($"1: {account.Owner} / {account.Balance}");

        // 2. 실패한 작업은 상태를 바꾸지 않아야 합니다.
        bool result = account.Deposit(0);
        Console.WriteLine($"2: {result} / {account.Balance}");

        // 3. private set으로 읽기는 허용하고 변경은 메서드에 모읍니다.
        Book book = new("C#");
        book.Rename("C# Guide");
        Console.WriteLine($"3: {book.Title}");
    }

    private sealed class Book
    {
        public string Title { get; private set; }

        public Book(string title)
        {
            Title = title;
        }

        public void Rename(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            Title = title;
        }
    }
}
