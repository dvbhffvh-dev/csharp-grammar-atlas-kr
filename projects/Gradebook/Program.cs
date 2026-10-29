using System.Globalization;
using System.Text;
using System.Text.Json;
using Gradebook;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--self-test"))
{
    await SelfTests.RunAsync();
    return;
}

if (args.Contains("--interactive"))
{
    await RunMenuAsync();
    return;
}

// 기본 실행은 입력 없이 끝나는 데모입니다. 직접 사용하려면 --interactive를 전달하세요.
GradebookService demo = new();
demo.Add("민수", 82);
demo.Add("지우", 96);
demo.Add("서연", 90);
Print(demo);
Console.WriteLine($"90점 이상: {demo.Ranked().Count(x => x.Score >= 90)}명");
Console.WriteLine($"검색 '민': {demo.Find("민")[0].Name}");
Console.WriteLine("직접 입력하려면 --interactive 옵션을 사용하세요.");

static void Print(GradebookService book)
{
    Console.WriteLine($"학생 {book.Count}명");
    foreach (StudentScore row in book.Ranked())
    {
        Console.WriteLine($"{row.Name}: {row.Score}점 ({GradebookService.Grade(row.Score)})");
    }
    Console.WriteLine(book.Average is double average ? $"평균: {average:F2}" : "평균: 데이터 없음");
}

static async Task RunMenuAsync()
{
    GradebookService book = new();
    string path = Path.Combine(Environment.CurrentDirectory, "data", "students.json");
    Console.WriteLine("성적 관리 | 저장 위치: " + path);
    Console.WriteLine("예제용 이름을 사용하세요. 파일 저장은 메뉴 5를 선택할 때만 실행됩니다.");
    while (true)
    {
        Console.WriteLine("1 추가 / 2 목록 / 3 검색 / 4 삭제 / 5 저장 / 6 불러오기 / 0 종료");
        string? choice = Console.ReadLine();
        if (choice is null or "0") return; // 입력 스트림 종료도 정상 종료로 처리합니다.
        try
        {
            switch (choice)
            {
                case "1":
                    Console.Write("이름: ");
                    string? name = Console.ReadLine();
                    Console.Write("점수(0~100): ");
                    if (!int.TryParse(Console.ReadLine(), out int score))
                    {
                        Console.WriteLine("정수를 입력하세요.");
                        break;
                    }
                    book.Add(name ?? "", score);
                    Console.WriteLine("추가했습니다.");
                    break;
                case "2":
                    Print(book);
                    break;
                case "3":
                    Console.Write("검색어: ");
                    foreach (StudentScore row in book.Find(Console.ReadLine() ?? ""))
                        Console.WriteLine($"{row.Name}: {row.Score}");
                    break;
                case "4":
                    Console.Write("삭제할 이름: ");
                    Console.WriteLine(book.Remove(Console.ReadLine() ?? "") ? "삭제했습니다." : "이름을 찾지 못했습니다.");
                    break;
                case "5":
                    await book.SaveAsync(path);
                    Console.WriteLine("저장했습니다.");
                    break;
                case "6":
                    Console.Write("현재 목록을 파일 내용으로 바꿀까요? (y): ");
                    if (Console.ReadLine() == "y")
                    {
                        await book.LoadAsync(path);
                        Console.WriteLine("불러왔습니다.");
                    }
                    break;
                default:
                    Console.WriteLine("메뉴 번호를 확인하세요.");
                    break;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or InvalidDataException or JsonException or UnauthorizedAccessException)
        {
            Console.WriteLine("처리할 수 없습니다: " + ex.Message);
        }
    }
}
