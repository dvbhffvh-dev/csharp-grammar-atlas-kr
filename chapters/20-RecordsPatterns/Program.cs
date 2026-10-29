if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

Point first = new(2, 3);
Point second = new(2, 3);
Console.WriteLine($"value-equal={first == second}");
Console.WriteLine($"same-instance={ReferenceEquals(first, second)}");

Book original = new("C#", 30, ["beginner"]);
Book revised = original with { Title = "C# Guide" };
revised.Tags[0] = "advanced";
Console.WriteLine($"titles={original.Title}/{revised.Title}");
Console.WriteLine($"shared-tag={original.Tags[0]}");
Console.WriteLine($"array-content-equality={new Book("A", 10, ["x"]) == new Book("A", 10, ["x"])}");

Console.WriteLine($"grade={Grade(92)}");
Console.WriteLine($"price={DescribePrice(original)}");
int[] sequence = [1, 2, 3, 4];
Console.WriteLine($"list-match={sequence is [1, .., 4]}");
Console.WriteLine($"unknown={Describe(new object())}");
Console.WriteLine($"null={Describe(null)}");

static string Grade(int score) => score switch
{
    < 0 or > 100 => "invalid",
    >= 90 => "A",
    >= 80 => "B",
    _ => "C"
};

static string DescribePrice(Book book) => book switch
{
    { Price: <= 30, Title.Length: > 0 } => "budget",
    _ => "standard"
};

static string Describe(object? value) => value switch
{
    null => "empty",
    int number => $"integer:{number}",
    string { Length: > 0 } text => $"text:{text}",
    _ => "other"
};

public sealed record Point(int X, int Y);
public sealed record Book(string Title, int Price, string[] Tags);
