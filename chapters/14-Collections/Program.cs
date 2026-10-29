if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

List<int> scores = new() { 30, 10, 20 };
scores.Add(40);
scores.Remove(10);
Console.WriteLine($"list: {string.Join(", ", scores)}");

Dictionary<string, int> stock = new(StringComparer.Ordinal)
{
    ["pen"] = 3,
    ["book"] = 2
};
stock["pen"] += 1;
bool found = stock.TryGetValue("pen", out int count);
Console.WriteLine($"dictionary: {found} / {count}");
Console.WriteLine($"missing: {stock.TryGetValue("eraser", out _)}");

HashSet<string> tags = new(StringComparer.Ordinal) { "C#", "dotnet" };
Console.WriteLine($"set add: {tags.Add("C#")} / {tags.Add("study")}");
Console.WriteLine($"set count: {tags.Count}");

Queue<string> waiting = new();
waiting.Enqueue("first");
waiting.Enqueue("second");
Stack<string> history = new();
history.Push("first");
history.Push("second");
Console.WriteLine($"order: {waiting.Dequeue()} / {history.Pop()}");

// foreach로 열거하는 동안 원본 목록을 변경하지 않습니다.
List<int> numbers = new() { 1, 2, 3, 4 };
for (int i = numbers.Count - 1; i >= 0; i--)
{
    if (numbers[i] % 2 == 0)
    {
        numbers.RemoveAt(i);
    }
}
Console.WriteLine($"filtered: {string.Join(", ", numbers)}");
