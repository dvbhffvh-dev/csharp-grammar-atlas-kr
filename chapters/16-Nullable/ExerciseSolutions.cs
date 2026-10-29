internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. null과 실제 0은 서로 다른 상태입니다.
        int? quantity = null;
        Console.WriteLine($"1: {quantity ?? 1}");

        // 2. ?.의 결과도 null일 수 있으므로 ??로 기본값을 정합니다.
        string? text = null;
        Console.WriteLine($"2: {text?.Trim().Length ?? 0}");

        // 3. 경계에서 유효성을 검사하고 내부에는 비-null 값을 전달합니다.
        Console.WriteLine($"3: {NormalizeName("  min  ")}");
        try
        {
            NormalizeName(null);
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("3: name required");
        }
    }

    private static string NormalizeName(string? name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return name.Trim().ToUpperInvariant();
    }
}
