if (args.Contains("--answers"))
{
    await ExerciseSolutions.RunAsync();
    return;
}

// Task.Delay는 외부 서비스 없이 비동기 대기를 재현하기 위한 장치다.
Task<string> first = LoadLabelAsync("A", 20, CancellationToken.None);
Task<string> second = LoadLabelAsync("B", 1, CancellationToken.None);
string[] labels = await Task.WhenAll(first, second);
// 완료 순서와 무관하게 WhenAll의 결과는 입력 작업 순서로 배치된다.
Console.WriteLine($"results={string.Join(",", labels)}");

int sum = await Task.Run(() => Enumerable.Range(1, 5).Sum());
Console.WriteLine($"cpu-sum={sum}");

using CancellationTokenSource source = new();
source.Cancel(); // 타이밍 경쟁 없이 취소 경로를 재현한다.
try
{
    await LoadLabelAsync("C", 1, source.Token);
}
catch (OperationCanceledException) when (source.IsCancellationRequested)
{
    Console.WriteLine("canceled=True");
}

try
{
    await FailAsync();
}
catch (InvalidOperationException error)
{
    Console.WriteLine($"error={error.Message}");
}

static async Task<string> LoadLabelAsync(string label, int delay, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    await Task.Delay(delay, cancellationToken);
    return $"loaded-{label}";
}

static async Task FailAsync()
{
    await Task.Yield();
    throw new InvalidOperationException("sample");
}
