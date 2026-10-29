internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 필터링 → 정렬 → 변환의 각 단계를 읽는다.
        int[] values = [5, 2, 8, 1, 4];
        int[] squares = values.Where(value => value % 2 == 0)
            .OrderBy(value => value).Select(value => value * value).ToArray();
        Console.WriteLine($"squares={string.Join(",", squares)}");

        // 2. 그룹 하나가 같은 분류에 속한 여러 원소를 보관한다.
        Student[] students = [new("Ada", "A", 90), new("Ben", "B", 70), new("Cora", "A", 80)];
        foreach (var team in students.GroupBy(student => student.Team).OrderBy(group => group.Key))
        {
            Console.WriteLine($"sum-{team.Key}={team.Sum(student => student.Score)}");
        }

        // 3. 원하는 시점에 평가해서 스냅샷을 만든다.
        List<int> source = [1, 2, 3];
        int[] fixedResult = source.Where(value => value > 1).ToArray();
        source.Add(4);
        Console.WriteLine($"fixed={string.Join(",", fixedResult)}");
        Console.WriteLine($"source={string.Join(",", source)}");
    }
}
