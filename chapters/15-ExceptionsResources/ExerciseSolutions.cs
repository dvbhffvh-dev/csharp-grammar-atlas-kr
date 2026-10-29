internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 예상 가능한 입력 실패는 TryParse로 표현할 수 있습니다.
        bool success = int.TryParse("oops", out int value);
        Console.WriteLine($"1: {success} / {value}");

        // 2. 예외가 발생해도 using의 정리 코드가 먼저 실행됩니다.
        try
        {
            using DemoResource resource = new("exercise");
            resource.Use();
            throw new InvalidOperationException("Exercise failure.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("2: recovered");
        }

        // 3. 메서드 반환 전에 using 선언의 자원이 정리됩니다.
        Console.WriteLine($"3: {ReadAnswer()}");
    }

    private static int ReadAnswer()
    {
        using DemoResource resource = new("answer");
        resource.Use();
        return 42;
    }
}
