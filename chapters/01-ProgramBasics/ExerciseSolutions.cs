public static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 한 문장씩 순서대로 출력합니다.
        Console.WriteLine("안녕하세요!");
        Console.WriteLine("C#을 배웁니다.");

        // 2. Write는 줄을 바꾸지 않습니다.
        Console.Write("이름: ");
        Console.WriteLine("지우");

        // 3. 입력이 없는 상황도 값으로 표현하여 재현합니다.
        string? simulatedInput = null;
        string name = string.IsNullOrWhiteSpace(simulatedInput)
            ? "방문자"
            : simulatedInput.Trim();
        Console.WriteLine($"안녕하세요, {name}!");
    }
}
