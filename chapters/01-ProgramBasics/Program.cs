if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 문장은 위에서 아래로 실행됩니다.
Console.WriteLine("C# 문법 아틀라스");
Console.Write("학습 ");
Console.WriteLine("시작");

// 예제 2: 값을 이름에 담은 뒤 출력합니다.
string learner = "민수";
int completed = 1;
Console.WriteLine($"학습자: {learner}");
Console.WriteLine($"완료 단원: {completed}");

// 예제 3: 기본 실행은 입력을 기다리지 않습니다.
// --interactive 옵션을 지정한 경우에만 실제 콘솔 입력을 받습니다.
if (args.Contains("--interactive"))
{
    Console.Write("이름을 입력하세요: ");
    string? input = Console.ReadLine();
    string name = string.IsNullOrWhiteSpace(input) ? "방문자" : input.Trim();
    Console.WriteLine($"안녕하세요, {name}!");
}
else
{
    Console.WriteLine("입력 예제: --interactive 옵션으로 실행");
}
