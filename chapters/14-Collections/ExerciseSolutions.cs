internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 정렬은 원본 List를 변경합니다.
        List<int> values = new() { 4, 1, 4, 2 };
        values.Sort();
        Console.WriteLine($"1: {string.Join(", ", values)}");

        // 2. 존재하지 않는 키의 개수를 0부터 누적합니다.
        string[] words = { "red", "blue", "red" };
        Dictionary<string, int> counts = new(StringComparer.Ordinal);
        foreach (string word in words)
        {
            counts.TryGetValue(word, out int previous);
            counts[word] = previous + 1;
        }
        Console.WriteLine($"2: red={counts["red"]}, blue={counts["blue"]}");

        // 3. 집합으로 발견 여부를 추적하고 결과는 List의 순서로 보관합니다.
        HashSet<int> seen = new();
        List<int> unique = new();
        foreach (int value in new[] { 3, 1, 3, 2, 1 })
        {
            if (seen.Add(value))
            {
                unique.Add(value);
            }
        }
        Console.WriteLine($"3: {string.Join(", ", unique)}");
    }
}
