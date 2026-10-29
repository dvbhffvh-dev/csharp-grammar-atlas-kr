using System.Globalization;
using System.Text;

public static class ExerciseSolutions
{
    public static void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        string rawName = "  MinSu  ";
        Console.WriteLine(rawName.Trim().ToLowerInvariant());

        string product = "노트";
        int quantity = 3;
        decimal unitPrice = 1200m;
        Console.WriteLine($"{product} {quantity}개: {unitPrice * quantity:F0}원");

        string[] words = { "C#", "문법", "연습" };
        StringBuilder builder = new();
        for (int i = 0; i < words.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(" | ");
            }
            builder.Append(words[i]);
        }
        Console.WriteLine(builder.ToString());
    }
}
