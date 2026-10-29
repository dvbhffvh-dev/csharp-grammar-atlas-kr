if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// C# 12: 컬렉션 식과 펼치기 요소. 대상 형식이 필요하다.
int[] first = [1, 2];
List<int> combined = [0, .. first, 3];
Console.WriteLine($"collection={string.Join(",", combined)}");

// C# 12: 일반 클래스의 주 생성자 매개변수는 자동 프로퍼티가 아니다.
Lesson lesson = new("LINQ", 19);
Console.WriteLine($"primary={lesson.Label}");

// C# 13: params의 대상이 배열 이외의 지원 컬렉션 형식으로 확장되었다.
Console.WriteLine($"params={Total(2, 3, 5)}");

// C# 14: field는 프로퍼티가 사용하는 컴파일러 생성 저장소를 가리킨다.
ReaderProfile profile = new() { Name = "  Ada  " };
Console.WriteLine($"field={profile.Name}");

// C# 14: 수신 객체가 null이면 대입식의 오른쪽도 실행되지 않는다.
ReaderProfile? missing = null;
int rightSideCalls = 0;
string BuildName()
{
    rightSideCalls++;
    return "Ben";
}
missing?.Name = BuildName();
Console.WriteLine($"skipped={rightSideCalls}");

ReaderProfile? present = profile;
present?.Name = BuildName();
Console.WriteLine($"assigned={profile.Name},calls={rightSideCalls}");

static int Total(params ReadOnlySpan<int> values)
{
    int total = 0;
    foreach (int value in values)
    {
        total += value;
    }
    return total;
}

public sealed class Lesson(string title, int number)
{
    public string Label { get; } = $"{number}:{title}";
}

public sealed class ReaderProfile
{
    public string Name
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Name must not be blank.", nameof(value))
            : value.Trim();
    } = "unknown";
}
