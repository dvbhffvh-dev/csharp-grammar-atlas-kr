using System.Globalization;

public static class ExerciseSolutions
{
    public static void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        string title = "C# 입문";
        int pages = 240;
        bool isBorrowed = false;
        Console.WriteLine($"{title}: {pages}쪽, 대출 중: {isBorrowed}");

        decimal unitPrice = 1500m;
        int quantity = 4;
        const decimal DiscountRate = 0.10m;
        decimal result = unitPrice * quantity * (1m - DiscountRate);
        Console.WriteLine($"할인 금액: {result:F0}");

        int? score = null;
        Console.WriteLine(score.HasValue ? $"점수: {score.Value}" : "미입력");
        score = 0;
        Console.WriteLine(score.HasValue ? $"점수: {score.Value}" : "미입력");
    }
}
