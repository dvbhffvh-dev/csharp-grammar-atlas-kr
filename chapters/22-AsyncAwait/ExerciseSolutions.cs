internal static class ExerciseSolutions
{
    public static async Task RunAsync()
    {
        // 1. Task<T>의 T를 await로 얻는다.
        Console.WriteLine($"double={await DoubleAsync(6, CancellationToken.None)}");

        // 2. 먼저 작업들을 만들고 함께 기다린다.
        Task<int>[] work = [DoubleAsync(2, CancellationToken.None), DoubleAsync(5, CancellationToken.None)];
        int[] results = await Task.WhenAll(work);
        Console.WriteLine($"batch={string.Join(",", results)}");

        // 3. 취소 토큰을 하위 작업까지 전달한다.
        using CancellationTokenSource source = new();
        source.Cancel();
        try
        {
            await DoubleAsync(9, source.Token);
        }
        catch (OperationCanceledException) when (source.IsCancellationRequested)
        {
            Console.WriteLine("answer-canceled");
        }
    }

    private static async Task<int> DoubleAsync(int value, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        return value * 2;
    }
}
