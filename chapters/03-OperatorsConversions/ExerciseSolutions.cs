using System.Globalization;

public static class ExerciseSolutions
{
    public static void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        int minutes = 135;
        Console.WriteLine($"{minutes / 60}시간 {minutes % 60}분");

        string text = "18";
        if (int.TryParse(text, out int age) && age >= 0 && age <= 150)
        {
            Console.WriteLine($"유효한 나이: {age}");
        }
        else
        {
            Console.WriteLine("나이를 다시 확인하세요.");
        }

        int unitPrice = 1_500_000_000;
        int quantity = 2;
        long amount = (long)unitPrice * quantity;
        Console.WriteLine($"총액: {amount}");
        // (long)(unitPrice * quantity)는 이미 int 계산을 끝낸 뒤 바꿉니다.
    }
}
