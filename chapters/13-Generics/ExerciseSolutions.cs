internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 타입 인수만 바꾸어 같은 제네릭 형식을 재사용합니다.
        Box<bool> flag = new(true);
        Console.WriteLine($"1: {flag.Value}");

        // 2. 호출 인수로 T=int를 추론합니다.
        int left = 2;
        int right = 8;
        Swap(ref left, ref right);
        Console.WriteLine($"2: {left} / {right}");

        // 3. 빈 입력의 의미를 계약에 명시합니다.
        int[] numbers = { 3, 9, 4 };
        Console.WriteLine($"3: {Greatest(numbers)}");
    }

    private static void Swap<T>(ref T left, ref T right)
    {
        T temporary = left;
        left = right;
        right = temporary;
    }

    private static T Greatest<T>(T[] values) where T : IComparable<T>
    {
        if (values.Length == 0)
        {
            throw new ArgumentException("At least one value is required.", nameof(values));
        }

        T best = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i].CompareTo(best) > 0)
            {
                best = values[i];
            }
        }

        return best;
    }
}
