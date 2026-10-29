internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 중간 컬렉션을 펼쳐 새 배열을 만든다.
        int[] middle = [2, 3];
        int[] all = [1, .. middle, 4];
        Console.WriteLine($"spread={string.Join(",", all)}");

        // 2. 빈 params 호출도 명시적으로 처리한다.
        Console.WriteLine($"sum={Sum(3, 7)}");
        Console.WriteLine($"empty-sum={Sum()}");

        // 3. 수신 객체 유무가 대입 실행 여부를 결정한다.
        ReaderProfile? profile = null;
        profile?.Name = "  Ignored  ";
        Console.WriteLine($"still-null={profile is null}");
        profile = new ReaderProfile();
        profile?.Name = "  Cora  ";
        Console.WriteLine($"trimmed={profile?.Name}");
    }

    private static int Sum(params ReadOnlySpan<int> values)
    {
        int result = 0;
        foreach (int value in values)
        {
            result += value;
        }
        return result;
    }
}
