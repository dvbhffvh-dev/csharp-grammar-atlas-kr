internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 입력과 반환형을 먼저 정한 뒤 람다 본문을 채운다.
        Func<int, int, int> max = static (left, right) => Math.Max(left, right);
        Console.WriteLine($"max={max(7, 4)}");

        // 2. 판단 정책을 매개변수로 전달한다.
        int[] values = [3, 8, 12, 5];
        Console.WriteLine($"filtered={string.Join(",", Filter(values, value => value >= 8))}");

        // 3. 각 반복에서 새로운 지역 변수를 만들어 독립적으로 캡처한다.
        List<Action> actions = [];
        for (int i = 1; i <= 3; i++)
        {
            int captured = i;
            actions.Add(() => Console.WriteLine($"item={captured}"));
        }
        actions.ForEach(action => action());
    }

    private static List<int> Filter(IEnumerable<int> source, Predicate<int> accept)
    {
        List<int> result = [];
        foreach (int item in source)
        {
            if (accept(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
}
