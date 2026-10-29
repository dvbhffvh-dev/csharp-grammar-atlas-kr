if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 값 형식의 대입은 값을 복사합니다.
int original = 10;
int copy = original;
copy = 20;
Console.WriteLine($"value: {original} / {copy}");

Position first = new() { X = 2 };
Position second = first;
second.X = 9;
Console.WriteLine($"struct: {first.X} / {second.X}");

// 예제 2: 참조를 복사하면 같은 객체를 가리킵니다.
Counter left = new() { Value = 3 };
Counter right = left;
right.Value = 8;
Console.WriteLine($"reference: {left.Value} / {right.Value}");
right = new Counter { Value = 99 };
Console.WriteLine($"replace: {left.Value} / {right.Value}");

// 예제 3: string은 참조 형식이지만 내용을 변경할 수 없습니다.
string word = "cat";
string other = word;
other += "!";
Console.WriteLine($"string: {word} / {other}");

// 예제 4: ref는 원래 변수 자체를 대상으로 작업합니다.
AddOne(ref original);
Console.WriteLine($"ref: {original}");

// enum은 이름이 붙은 정수 상수를 정의하는 값 형식입니다.
OrderState state = OrderState.Paid;
Console.WriteLine($"enum: {state} / {(int)state}");

static void AddOne(ref int value)
{
    value++;
}

internal struct Position
{
    public int X;
}

internal sealed class Counter
{
    public int Value { get; set; }
}

internal enum OrderState
{
    Pending = 0,
    Paid = 1,
    Cancelled = 2
}
