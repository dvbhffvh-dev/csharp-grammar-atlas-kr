using System.Text.Json;

namespace Gradebook;

// JSON에는 데이터만 기록합니다. 검증과 계산은 서비스가 담당합니다.
public sealed record StudentScore(string Name, int Score);

public sealed class GradebookService
{
    private readonly List<StudentScore> students = [];

    public int Count => students.Count;
    public double? Average => students.Count == 0 ? null : students.Average(x => x.Score);

    public void Add(string name, int score)
    {
        StudentScore candidate = Validate(name, score);
        if (students.Any(x => string.Equals(x.Name, candidate.Name, StringComparison.Ordinal)))
        {
            throw new ArgumentException("같은 이름이 이미 있습니다.", nameof(name));
        }
        students.Add(candidate);
    }

    // 새 배열을 돌려주므로 호출자가 내부 List를 직접 수정하지 못합니다.
    public StudentScore[] Ranked() => students
        .OrderByDescending(x => x.Score)
        .ThenBy(x => x.Name, StringComparer.Ordinal)
        .ToArray();

    public StudentScore[] Find(string keyword) => students
        .Where(x => x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        .ToArray();

    public bool Remove(string name) => students.RemoveAll(x => x.Name == name.Trim()) > 0;

    public static string Grade(int score) => score switch
    {
        < 0 or > 100 => throw new ArgumentOutOfRangeException(nameof(score)),
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };

    public async Task SaveAsync(string path, CancellationToken token = default)
    {
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        // 임시 파일을 완성한 뒤 이동합니다. 직렬화 실패로 기존 파일을 비우는 일을 피합니다.
        string temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, students,
                    new JsonSerializerOptions { WriteIndented = true }, token);
            }
            token.ThrowIfCancellationRequested();
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }

    public async Task LoadAsync(string path, CancellationToken token = default)
    {
        await using FileStream stream = File.OpenRead(path);
        List<StudentScore?>? loaded = await JsonSerializer.DeserializeAsync<List<StudentScore?>>(stream,
            cancellationToken: token);
        if (loaded is null) throw new InvalidDataException("학생 목록이 없습니다.");

        // 모두 검증하기 전에는 기존 목록을 건드리지 않습니다.
        List<StudentScore> validated = [];
        HashSet<string> names = new(StringComparer.Ordinal);
        foreach (StudentScore? row in loaded)
        {
            if (row is null) throw new InvalidDataException("비어 있는 학생 항목입니다.");
            StudentScore candidate = Validate(row.Name, row.Score);
            if (!names.Add(candidate.Name)) throw new InvalidDataException("이름이 중복됩니다.");
            validated.Add(candidate);
        }
        students.Clear();
        students.AddRange(validated);
    }

    private static StudentScore Validate(string? name, int score)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("이름을 입력하세요.", nameof(name));
        if (score is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(score), "점수는 0~100입니다.");
        return new StudentScore(name.Trim(), score);
    }
}
