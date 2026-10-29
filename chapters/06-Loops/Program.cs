if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 초기화 → 조건 → 본문 → 증감 → 조건 순서입니다.
int sum = 0;
for (int number = 1; number <= 5; number++)
{
    sum += number;
}
Console.WriteLine($"1~5 합: {sum}");

// 예제 2: while은 실행 전에, do-while은 실행 뒤에 검사합니다.
int remaining = 3;
while (remaining > 0)
{
    Console.WriteLine($"남은 횟수: {remaining}");
    remaining--;
}
int attempts = 0;
do
{
    attempts++;
}
while (attempts < 1);
Console.WriteLine($"do-while 실행: {attempts}");

// 예제 3: continue는 현재 회차를 건너뛰고 break는 반복을 끝냅니다.
int selectedSum = 0;
for (int number = 1; number <= 10; number++)
{
    if (number == 8)
    {
        break;
    }
    if (number % 2 != 0)
    {
        continue;
    }
    selectedSum += number;
}
Console.WriteLine($"8 이전 짝수 합: {selectedSum}");

// 예제 4: foreach는 요소를 순서대로 읽습니다.
string[] topics = { "변수", "조건", "반복" };
foreach (string topic in topics)
{
    Console.WriteLine($"복습: {topic}");
}

// 중첩 반복문은 바깥 반복 한 번마다 안쪽 반복을 끝까지 수행합니다.
for (int row = 1; row <= 2; row++)
{
    for (int column = 1; column <= 3; column++)
    {
        if (column > 1)
        {
            Console.Write(" ");
        }
        Console.Write(row * column);
    }
    Console.WriteLine();
}
