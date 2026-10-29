if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

IEnumerable<int> sequence = Generate();
Console.WriteLine("created");
foreach (int value in sequence)
{
    Console.WriteLine($"value={value}");
    break; // foreach는 열거자를 Dispose하여 iterator의 finally를 실행한다.
}

Console.WriteLine($"extension={12.ClampTo(0, 10)}");
(int minimum, int maximum) = MinMax([7, 2, 9]);
Console.WriteLine($"tuple={minimum},{maximum}");

string[] words = ["zero", "one", "two", "three"];
Console.WriteLine($"last={words[^1]}");
Console.WriteLine($"slice={string.Join(",", words[1..3])}");

Shelf shelf = new(["C#", "LINQ"]);
Console.WriteLine($"indexer={shelf[1]}");
shelf[1] = "Async";
Console.WriteLine($"updated={shelf[1]}");

static IEnumerable<int> Generate()
{
    try
    {
        Console.WriteLine("iterator-start");
        yield return 10;
        yield return 20;
    }
    finally
    {
        Console.WriteLine("iterator-disposed");
    }
}

static (int Min, int Max) MinMax(int[] values)
{
    if (values.Length == 0)
    {
        throw new ArgumentException("At least one value is required.", nameof(values));
    }
    return (values.Min(), values.Max());
}

public static class NumberExtensions
{
    public static int ClampTo(this int value, int minimum, int maximum)
        => Math.Clamp(value, minimum, maximum);
}

public sealed class Shelf(string[] books)
{
    private readonly string[] _books = [.. books];

    public string this[int index]
    {
        get => _books[index];
        set => _books[index] = value;
    }
}
