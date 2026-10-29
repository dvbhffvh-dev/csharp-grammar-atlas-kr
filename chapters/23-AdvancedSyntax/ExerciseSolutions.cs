internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 필요한 원소 수만 소비하면 나머지를 생성하지 않는다.
        Console.WriteLine($"even={string.Join(",", EvenNumbers().Take(4))}");

        // 2. 튜플을 반환하고 필요 없는 요소는 버린다.
        (int count, int total) = Summarize([2, 4, 6]);
        Console.WriteLine($"summary={count},{total}");
        var (_, anotherTotal) = Summarize([1, 3]);
        Console.WriteLine($"discard={anotherTotal}");

        // 3. 배열 범위 결과는 원소들을 담은 별도 배열이다.
        int[] original = [10, 20, 30, 40];
        int[] middle = original[1..^1];
        middle[0] = 99;
        Console.WriteLine($"middle={string.Join(",", middle)}");
        Console.WriteLine($"original={string.Join(",", original)}");
    }

    private static IEnumerable<int> EvenNumbers()
    {
        for (int value = 0; value <= int.MaxValue - 2; value += 2)
        {
            yield return value;
        }
    }

    private static (int Count, int Sum) Summarize(int[] values) => (values.Length, values.Sum());
}
