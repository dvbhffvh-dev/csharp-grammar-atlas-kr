if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// Action은 결과가 없는 동작, Func는 마지막 형식이 반환형인 함수다.
Action<string> log = Console.WriteLine;
Func<int, int> square = static value => value * value;
Predicate<int> isEven = static value => value % 2 == 0;
log($"square(3)={square(3)}");
log($"even(8)={isEven(8)}");

// 캡처는 생성 시점의 값 복사가 아니라 바깥 변수에 대한 접근이다.
int threshold = 10;
Func<int, bool> isLarge = value => value > threshold;
log($"before={isLarge(12)}");
threshold = 20;
log($"after={isLarge(12)}");

List<Func<int>> shared = [];
List<Func<int>> copied = [];
for (int i = 0; i < 3; i++)
{
    shared.Add(() => i);
    int current = i;
    copied.Add(() => current);
}
log($"shared={string.Join(",", shared.Select(read => read()))}");
log($"copied={string.Join(",", copied.Select(read => read()))}");

Action notify = () => log("first");
notify += () => log("second");
notify();
