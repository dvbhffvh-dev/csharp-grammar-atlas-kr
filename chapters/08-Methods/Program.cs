using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 이름 있는 인수와 기본값은 호출 의도를 분명하게 합니다.
Console.WriteLine(MathExamples.Greet("지우"));
Console.WriteLine(MathExamples.Greet(suffix: "님", name: "민수"));
Console.WriteLine($"합계: {MathExamples.Add(2, 3)}");
Console.WriteLine($"실수 합계: {MathExamples.Add(2.5, 3.5):F1}");

// 예제 2: 일반 인수는 복사되고 ref는 호출자 변수를 직접 바꿉니다.
int value = 10;
MathExamples.IncreaseCopy(value);
Console.WriteLine($"일반 전달 후: {value}");
MathExamples.Increase(ref value);
Console.WriteLine($"ref 전달 후: {value}");

// 예제 3: out은 추가 결과를, in은 읽기 전용 참조 전달을 표현합니다.
bool success = MathExamples.TryDivide(17, 5, out int quotient, out int remainder);
Console.WriteLine($"나눗셈 성공: {success}, 몫: {quotient}, 나머지: {remainder}");
int number = 4;
Console.WriteLine($"in 제곱: {MathExamples.Square(in number)}");
Console.WriteLine($"params 합: {MathExamples.Sum(1, 2, 3, 4)}");

// 예제 4: 종료 조건과 입력 범위를 둔 재귀입니다.
Console.WriteLine($"5! = {MathExamples.Factorial(5)}");

public static class MathExamples
{
    public static string Greet(string name, string suffix = "씨")
    {
        return $"안녕하세요, {name}{suffix}!";
    }

    public static int Add(int left, int right) => left + right;
    public static double Add(double left, double right) => left + right;

    public static void IncreaseCopy(int value)
    {
        value++;
    }

    public static void Increase(ref int value)
    {
        value++;
    }

    public static bool TryDivide(int dividend, int divisor, out int quotient, out int remainder)
    {
        quotient = 0;
        remainder = 0;
        if (divisor == 0 || (dividend == int.MinValue && divisor == -1))
        {
            return false;
        }
        quotient = dividend / divisor;
        remainder = dividend % divisor;
        return true;
    }

    public static int Square(in int value) => checked(value * value);

    public static int Sum(params int[] numbers)
    {
        int sum = 0;
        foreach (int number in numbers)
        {
            sum = checked(sum + number);
        }
        return sum;
    }

    public static long Factorial(int number)
    {
        if (number < 0 || number > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "0~20만 지원합니다.");
        }
        if (number <= 1)
        {
            return 1;
        }
        return checked(number * Factorial(number - 1));
    }
}
