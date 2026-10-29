using System.Text.Json;

internal static class ExerciseSolutions
{
    public static void Run()
    {
        DirectoryInfo ownedDirectory = Directory.CreateTempSubdirectory("csharp-atlas-21-answer-");
        try
        {
            // 1. 경로 구분자를 직접 붙이지 않는다.
            string path = Path.Combine(ownedDirectory.FullName, "practice.txt");
            File.WriteAllText(path, "practice");
            Console.WriteLine($"text={File.ReadAllText(path)}");

            // 2. 직렬화와 역직렬화로 자료 구조를 왕복한다.
            StudyProfile original = new("Ben", [4, 5]);
            string json = JsonSerializer.Serialize(original);
            StudyProfile restored = JsonSerializer.Deserialize<StudyProfile>(json)
                ?? throw new InvalidDataException("Expected a profile.");
            Console.WriteLine($"restored={restored.Name}:{restored.CompletedChapters.Length}");

            // 3. JSON 문법 오류와 정상 JSON null을 구분한다.
            try
            {
                _ = JsonSerializer.Deserialize<StudyProfile>("{ invalid }");
            }
            catch (JsonException)
            {
                Console.WriteLine("invalid-json");
            }

            StudyProfile? empty = JsonSerializer.Deserialize<StudyProfile>("null");
            Console.WriteLine($"json-null={empty is null}");
        }
        finally
        {
            ownedDirectory.Delete(recursive: true);
        }
    }
}
