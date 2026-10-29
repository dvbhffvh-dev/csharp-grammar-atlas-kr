if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

Box<int> number = new(42);
Box<string> word = new("atlas");
Console.WriteLine($"box: {number.Value} / {word.Value}");
Console.WriteLine($"echo: {GenericTools.Echo(7)} / {GenericTools.Echo("go")}");
Console.WriteLine($"max: {GenericTools.Max(4, 9)}");
Console.WriteLine($"default: {GenericTools.Zero<int>()} / {GenericTools.Zero<bool>()}");
Ticket ticket = GenericTools.Create<Ticket>();
Console.WriteLine($"new: {ticket.Name}");

internal sealed class Box<T>
{
    public T Value { get; }

    public Box(T value)
    {
        Value = value;
    }
}

internal static class GenericTools
{
    public static T Echo<T>(T value) => value;

    public static T Max<T>(T left, T right) where T : IComparable<T>
        => left.CompareTo(right) >= 0 ? left : right;

    public static T Zero<T>() where T : struct => default;

    public static T Create<T>() where T : new() => new T();
}

internal sealed class Ticket
{
    public string Name { get; } = "ticket";

    public Ticket()
    {
    }
}
