if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

Student[] students =
[
    new("Cora", "red", 90),
    new("Ben", "blue", 70),
    new("Ada", "blue", 95)
];

string[] names = students
    .Where(student => student.Score >= 80)
    .OrderBy(student => student.Name, StringComparer.Ordinal)
    .Select(student => student.Name)
    .ToArray();
Console.WriteLine($"passed={string.Join(",", names)}");

foreach (var group in students.GroupBy(student => student.Team)
    .OrderBy(group => group.Key, StringComparer.Ordinal))
{
    Console.WriteLine($"team={group.Key},count={group.Count()}");
}

Team[] teams = [new("blue", "Ocean"), new("red", "Fire")];
var joined = students.Join(teams, student => student.Team, team => team.Id,
    (student, team) => $"{student.Name}:{team.Title}");
Console.WriteLine($"joined={string.Join(",", joined)}");
Console.WriteLine($"perfect={students.Any(student => student.Score == 100)}");

List<int> numbers = [1, 2, 3];
IEnumerable<int> live = numbers.Where(number => number >= 3);
numbers.Add(4);
int[] snapshot = live.ToArray();
numbers.Add(5);
Console.WriteLine($"snapshot={string.Join(",", snapshot)}");
Console.WriteLine($"live={string.Join(",", live)}");

// 관찰용 부작용이다. 실제 조건식은 가능하면 부작용 없이 작성한다.
int predicateCalls = 0;
var observed = numbers.Where(number =>
{
    predicateCalls++;
    return number % 2 == 0;
});
_ = observed.Count();
_ = observed.Count();
Console.WriteLine($"predicate-calls={predicateCalls}");

public sealed record Student(string Name, string Team, int Score);
public sealed record Team(string Id, string Title);
