public static class ExerciseSolutions
{
    public static void Run()
    {
        Console.WriteLine($"최댓값: {Max(3, 8)}");
        int first = 10;
        int second = 20;
        Swap(ref first, ref second);
        Console.WriteLine($"교환: {first}, {second}");
        Console.WriteLine($"최대공약수: {GreatestCommonDivisor(48, 18)}");
    }

    public static int Max(int left, int right) => left > right ? left : right;

    public static void Swap(ref int left, ref int right)
    {
        int temporary = left;
        left = right;
        right = temporary;
    }

    // 음수 입력의 절댓값 경계 문제를 피하려고 양수만 허용합니다.
    public static int GreatestCommonDivisor(int left, int right)
    {
        if (left <= 0 || right <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(left), "두 값은 양수여야 합니다.");
        }

        // 재귀를 쓰지 않아 입력 크기에 따라 호출 스택이 쌓이지 않습니다.
        while (right != 0)
        {
            int remainder = left % right;
            left = right;
            right = remainder;
        }
        return left;
    }
}
