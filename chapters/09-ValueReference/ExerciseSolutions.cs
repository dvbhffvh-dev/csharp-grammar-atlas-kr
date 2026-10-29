internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 배열 자체는 참조 형식입니다. Clone은 새로운 배열을 만듭니다.
        int[] original = { 1, 2 };
        int[] copied = (int[])original.Clone();
        copied[0] = 9;
        Console.WriteLine($"1: {original[0]} / {copied[0]}");

        // 2. 참조를 값으로 전달해도 객체 내부의 변경은 호출자에게 보입니다.
        Counter counter = new() { Value = 5 };
        ChangeCounter(counter);
        Console.WriteLine($"2: {counter.Value}");

        // 3. 두 호출자 변수를 바꿔야 하므로 ref를 사용합니다.
        int left = 4;
        int right = 7;
        Swap(ref left, ref right);
        Console.WriteLine($"3: {left} / {right}");
    }

    private static void ChangeCounter(Counter counter)
    {
        counter.Value = 12;
        counter = new Counter { Value = 99 };
        // 위 대입은 매개변수의 참조만 교체합니다.
    }

    private static void Swap(ref int left, ref int right)
    {
        int temporary = left;
        left = right;
        right = temporary;
    }
}
