using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 피연산자의 타입이 나눗셈 동작을 정합니다.
int total = 7;
int people = 2;
Console.WriteLine($"정수 나눗셈: {total / people}");
Console.WriteLine($"실수 나눗셈: {(double)total / people:F1}");
Console.WriteLine($"나머지: {total % people}");

// 예제 2: 입력 변환의 성공 여부를 먼저 검사합니다.
string input = "42";
bool parsed = int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number);
Console.WriteLine($"변환 성공: {parsed}, 값: {number}");
bool invalid = int.TryParse("사십이", out int invalidNumber);
Console.WriteLine($"잘못된 입력: {invalid}, 결과: {invalidNumber}");

// 예제 3: &&는 왼쪽이 false면 오른쪽을 실행하지 않습니다.
int checks = 0;
bool left = false;
bool shortCircuit = left && ++checks > 0;
Console.WriteLine($"&& 결과: {shortCircuit}, 검사 횟수: {checks}");
bool fullEvaluation = left & ++checks > 0;
Console.WriteLine($"& 결과: {fullEvaluation}, 검사 횟수: {checks}");

// 예제 4: 범위를 벗어난 정수 계산을 명시적으로 검출합니다.
int largest = int.MaxValue;
try
{
    int overflowed = checked(largest + 1);
    Console.WriteLine(overflowed);
}
catch (OverflowException)
{
    Console.WriteLine("checked: 정수 범위를 초과했습니다.");
}

double measured = 3.9;
int truncated = (int)measured;
Console.WriteLine($"명시적 변환: {truncated}");
