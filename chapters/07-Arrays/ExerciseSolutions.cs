using System.Globalization;

public static class ExerciseSolutions
{
    public static void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        int[] values = { 5, 2, 9, 4 };
        int largest = values[0]; // 이 문제는 비어 있지 않은 배열을 전제합니다.
        foreach (int value in values)
        {
            if (value > largest)
            {
                largest = value;
            }
        }
        Console.WriteLine($"최댓값: {largest}");

        int[] reversed = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            reversed[i] = values[values.Length - 1 - i];
        }
        Console.WriteLine($"뒤집기: {string.Join(", ", reversed)}");

        int[][] scores = { new[] { 80, 100 }, Array.Empty<int>(), new[] { 60, 70, 80 } };
        for (int row = 0; row < scores.Length; row++)
        {
            if (scores[row].Length == 0)
            {
                Console.WriteLine($"학생 {row + 1}: 점수 없음");
                continue;
            }
            int sum = 0;
            foreach (int score in scores[row])
            {
                sum += score;
            }
            Console.WriteLine($"학생 {row + 1}: {(double)sum / scores[row].Length:F1}");
        }
    }
}
