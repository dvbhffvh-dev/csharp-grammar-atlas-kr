if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 인터페이스 형식으로 구현을 교체합니다.
IFormatter formatter = new UpperFormatter();
Console.WriteLine($"format: {formatter.Format("hello")}");

// 예제 2: 서비스는 서식 기능을 상속받지 않고 보유합니다.
ReportService first = new(new UpperFormatter());
ReportService second = new(new BracketFormatter());
Console.WriteLine($"report: {first.Create("study")}");
Console.WriteLine($"report: {second.Create("study")}");

// 예제 3: 같은 계약의 여러 구현을 같은 방식으로 사용합니다.
IFormatter[] formatters = { new UpperFormatter(), new BracketFormatter() };
foreach (IFormatter item in formatters)
{
    Console.WriteLine($"loop: {item.Format("csharp")}");
}

internal interface IFormatter
{
    string Format(string text);
}

internal sealed class UpperFormatter : IFormatter
{
    public string Format(string text) => text.ToUpperInvariant();
}

internal sealed class BracketFormatter : IFormatter
{
    public string Format(string text) => $"[{text}]";
}

internal sealed class ReportService
{
    private readonly IFormatter _formatter;

    public ReportService(IFormatter formatter)
    {
        _formatter = formatter;
    }

    public string Create(string title) => _formatter.Format(title);
}
