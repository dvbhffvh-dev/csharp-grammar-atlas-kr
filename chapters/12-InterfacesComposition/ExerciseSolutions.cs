internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 새 구현은 같은 메서드 계약을 지킵니다.
        IFormatter formatter = new PrefixFormatter();
        Console.WriteLine($"1: {formatter.Format("note")}");

        // 2. ReportService 코드는 수정할 필요가 없습니다.
        ReportService report = new(formatter);
        Console.WriteLine($"2: {report.Create("plan")}");

        // 3. 다른 formatter를 내부에 보유하여 기능을 합성합니다.
        IFormatter combined = new DoubleFormatter(
            new UpperFormatter(), new BracketFormatter());
        Console.WriteLine($"3: {combined.Format("book")}");
    }

    private sealed class PrefixFormatter : IFormatter
    {
        public string Format(string text) => $"title: {text}";
    }

    private sealed class DoubleFormatter : IFormatter
    {
        private readonly IFormatter _first;
        private readonly IFormatter _second;

        public DoubleFormatter(IFormatter first, IFormatter second)
        {
            _first = first;
            _second = second;
        }

        public string Format(string text) => _second.Format(_first.Format(text));
    }
}
