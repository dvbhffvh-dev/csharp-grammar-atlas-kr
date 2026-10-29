if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 값 형식의 nullable은 실제 Nullable<T> 형식입니다.
int? score = null;
Console.WriteLine($"value: {score.HasValue} / {score ?? -1}");
score = 80;
if (score is int actual)
{
    Console.WriteLine($"present: {actual}");
}

// 참조 형식의 ?는 컴파일러의 null 분석을 위한 표기입니다.
string? name = null;
Console.WriteLine($"length: {name?.Length ?? 0}");
name ??= "Guest";
Console.WriteLine($"name: {name}");

string? found = FindName(1);
if (found is not null)
{
    Console.WriteLine($"found: {found.ToUpperInvariant()}");
}

// !는 null을 제거하거나 값을 검증하지 않습니다.
string? missing = FindName(99);
string claimed = missing!;
Console.WriteLine($"forgiving still null: {claimed is null}");

static string? FindName(int id) => id == 1 ? "min" : null;
