namespace Gradebook;

// 외부 테스트 패키지 없이 핵심 동작을 확인하는 작은 실행형 검증 예제입니다.
internal static class SelfTests
{
    public static async Task RunAsync()
    {
        GradebookService book = new();
        Check(book.Count == 0 && book.Average is null, "빈 목록");
        book.Add("민수", 80);
        book.Add("서연", 100);
        Check(book.Average == 90 && book.Ranked()[0].Name == "서연", "평균과 정렬");
        Expect<ArgumentException>(() => book.Add(" 민수 ", 30));
        Expect<ArgumentOutOfRangeException>(() => book.Add("지우", 101));
        Expect<ArgumentException>(() => book.Add("  ", 30));
        Check(book.Count == 2, "실패한 추가는 상태를 변경하지 않음");
        Check(GradebookService.Grade(90) == "A" && GradebookService.Grade(89) == "B" &&
            GradebookService.Grade(0) == "F" && GradebookService.Grade(100) == "A", "등급 경계");
        Check(book.Find("민").Length == 1, "부분 검색");

        string directory = Path.Combine(Path.GetTempPath(), "csharp-gradebook-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "test.json");
            await book.SaveAsync(path);
            GradebookService copy = new();
            await copy.LoadAsync(path);
            Check(copy.Ranked().SequenceEqual(book.Ranked()), "JSON 왕복");
            await File.WriteAllTextAsync(path, "[{\"Name\":\"오류\",\"Score\":200}]");
            try
            {
                await copy.LoadAsync(path);
                throw new InvalidOperationException("검증 실패: 잘못된 점수를 받아들임");
            }
            catch (ArgumentOutOfRangeException) { }
            Check(copy.Count == 2 && copy.Average == 90, "잘못된 파일은 기존 목록 보존");
            await File.WriteAllTextAsync(path, "[{\"Name\":\"민수\",\"Score\":1},{\"Name\":\"민수\",\"Score\":2}]");
            try
            {
                await copy.LoadAsync(path);
                throw new InvalidOperationException("검증 실패: 중복 이름을 받아들임");
            }
            catch (InvalidDataException) { }
            Check(copy.Count == 2, "중복 파일은 기존 목록 보존");
        }
        finally
        {
            Directory.Delete(directory, recursive: true); // 이 실행에서 만든 임시 폴더만 삭제합니다.
        }
        Check(book.Remove("민수") && !book.Remove("없는이름") && book.Count == 1, "삭제");
        Console.WriteLine("Gradebook self-tests: PASS");
    }

    private static void Check(bool condition, string description)
    {
        if (!condition) throw new InvalidOperationException("검증 실패: " + description);
    }

    private static void Expect<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException($"검증 실패: {typeof(TException).Name} 예외가 필요합니다.");
    }
}
