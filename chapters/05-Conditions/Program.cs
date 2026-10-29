if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 범위가 겹치는 조건은 구체적인 조건부터 검사합니다.
int score = 87;
string grade;
if (score >= 90)
{
    grade = "A";
}
else if (score >= 80)
{
    grade = "B";
}
else if (score >= 70)
{
    grade = "C";
}
else
{
    grade = "재도전";
}
Console.WriteLine($"점수: {score}, 등급: {grade}");

// 예제 2: 조건을 이름 붙이면 복합 판단을 읽기 쉬워집니다.
int age = 19;
bool hasTicket = true;
bool canEnter = age >= 18 && hasTicket;
Console.WriteLine(canEnter ? "입장 가능" : "입장 불가");

// 예제 3: switch 문은 하나의 값에 따른 여러 동작을 고릅니다.
string command = "save";
switch (command)
{
    case "open":
        Console.WriteLine("열기 명령");
        break;
    case "save":
        Console.WriteLine("저장 명령");
        break;
    case "quit":
    case "exit":
        Console.WriteLine("종료 명령");
        break;
    default:
        Console.WriteLine("알 수 없는 명령");
        break;
}

// 예제 4: 조건 연산자는 값을 선택하는 식입니다.
int quantity = 3;
int shipping = quantity >= 3 ? 0 : 3000;
Console.WriteLine($"배송비: {shipping}");

int invalidScore = -1;
if (invalidScore < 0 || invalidScore > 100)
{
    Console.WriteLine("점수는 0~100이어야 합니다.");
}
