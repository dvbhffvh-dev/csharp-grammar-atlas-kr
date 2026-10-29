using System.Globalization;
using System.Text;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 이스케이프와 축자 문자열은 같은 값을 표현할 수 있습니다.
string escaped = "C:\\Study\\CSharp";
string verbatim = @"C:\Study\CSharp";
Console.WriteLine($"경로 일치: {escaped == verbatim}");
Console.WriteLine("따옴표: \"안녕\"");

// 예제 2: 보간으로 값을 서식과 함께 넣습니다.
string learner = "지우";
decimal price = 1234.5m;
Console.WriteLine($"{learner}의 가격: {price:F2}");

// 예제 3: raw string literal은 C# 11부터 지원됩니다.
string json = """{"name":"지우","level":1}""";
Console.WriteLine($"JSON: {json}");

// 예제 4: 문자열은 불변이며 가공 결과를 새 값으로 반환합니다.
string original = "  csharp  ";
string normalized = original.Trim().ToUpperInvariant();
Console.WriteLine($"원본 길이: {original.Length}, 정리: {normalized}");
Console.WriteLine($"접두사 검사: {normalized.StartsWith("CS", StringComparison.Ordinal)}");

string[] topics = "변수,조건,반복".Split(',');
Console.WriteLine($"합치기: {string.Join(" / ", topics)}");

StringBuilder builder = new();
builder.Append("C#");
builder.Append(' ');
builder.Append("학습");
Console.WriteLine($"빌더: {builder}");

string emoji = "😀";
Console.WriteLine($"이모지 UTF-16 길이: {emoji.Length}");
