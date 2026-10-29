using System.Text;
using System.Text.Json;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// OS가 만든 이번 실행 전용 임시 디렉터리만 사용한다.
DirectoryInfo ownedDirectory = Directory.CreateTempSubdirectory("csharp-atlas-21-");
try
{
    string textPath = Path.Combine(ownedDirectory.FullName, "notes.txt");
    using (StreamWriter writer = new(textPath, false, new UTF8Encoding(false)))
    {
        writer.WriteLine("C#");
        writer.WriteLine("JSON");
    } // Dispose에서 버퍼를 기록하고 파일 핸들을 반환한다.

    using (StreamReader reader = new(textPath, Encoding.UTF8))
    {
        Console.WriteLine($"first-line={reader.ReadLine()}");
        Console.WriteLine($"second-line={reader.ReadLine()}");
    }

    StudyProfile profile = new("Ada", [1, 2, 3]);
    string json = JsonSerializer.Serialize(profile);
    string jsonPath = Path.Combine(ownedDirectory.FullName, "profile.json");
    File.WriteAllText(jsonPath, json, new UTF8Encoding(false));

    StudyProfile loaded = JsonSerializer.Deserialize<StudyProfile>(File.ReadAllText(jsonPath))
        ?? throw new InvalidDataException("Profile JSON must not be null.");
    Console.WriteLine($"json={json}");
    Console.WriteLine($"name={loaded.Name}");
    Console.WriteLine($"completed={string.Join(",", loaded.CompletedChapters)}");
    Console.WriteLine($"file={Path.GetFileName(jsonPath)}");
}
finally
{
    // 파일 경로를 외부에서 받지 않으며 생성한 전용 디렉터리만 정리한다.
    ownedDirectory.Delete(recursive: true);
}
Console.WriteLine($"cleaned={!Directory.Exists(ownedDirectory.FullName)}");

public sealed record StudyProfile(string Name, int[] CompletedChapters);
