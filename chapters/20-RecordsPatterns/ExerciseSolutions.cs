internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. with로 변경본을 만들고 원본의 값을 확인한다.
        Point original = new(1, 2);
        Point moved = original with { X = 5 };
        Console.WriteLine($"points={original.X},{original.Y}/{moved.X},{moved.Y}");

        // 2. 관계 패턴의 경계와 잘못된 값을 각각 처리한다.
        foreach (int age in new[] { -1, 12, 20, 70 })
        {
            Console.WriteLine($"age-{age}={Category(age)}");
        }

        // 3. 중첩 배열도 복사하여 참조 공유를 끊는다.
        Book first = new("C#", 20, ["basic"]);
        Book independent = first with { Tags = [.. first.Tags] };
        independent.Tags[0] = "changed";
        Console.WriteLine($"tags={first.Tags[0]}/{independent.Tags[0]}");
    }

    private static string Category(int age) => age switch
    {
        < 0 => "invalid",
        < 18 => "child",
        < 65 => "adult",
        _ => "senior"
    };
}
