public static class ExerciseSolutions
{
    public static void Run()
    {
        int sum = 0;
        for (int number = 1; number <= 10; number++)
        {
            sum += number;
        }
        Console.WriteLine($"합: {sum}");

        for (int number = 1; number <= 12; number++)
        {
            if (number % 3 == 0)
            {
                continue;
            }
            Console.Write($"{number} ");
        }
        Console.WriteLine();

        int[] values = { 3, 5, 7, 9, 11 };
        int found = -1;
        foreach (int value in values)
        {
            if (value > 6)
            {
                found = value;
                break;
            }
        }
        Console.WriteLine($"처음 찾은 값: {found}");
    }
}
