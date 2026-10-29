if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 예제 1: 예외를 잡아도 finally를 거칩니다.
try
{
    Console.WriteLine("try: start");
    Console.WriteLine(10 / ReadZero());
}
catch (DivideByZeroException)
{
    Console.WriteLine("catch: divide by zero");
}
finally
{
    Console.WriteLine("finally: cleanup");
}

// 예제 2: using 블록을 벗어날 때 Dispose가 실행됩니다.
using (DemoResource resource = new("sample"))
{
    resource.Use();
}
Console.WriteLine("after using");

// 예제 3: throw;는 원래 예외를 다시 전달합니다.
try
{
    ValidateAndRethrow();
}
catch (ArgumentOutOfRangeException)
{
    Console.WriteLine("outer: invalid quantity");
}

static int ReadZero() => 0;

static void ValidateAndRethrow()
{
    try
    {
        ArgumentOutOfRangeException.ThrowIfNegative(-1, "quantity");
    }
    catch (ArgumentOutOfRangeException)
    {
        Console.WriteLine("inner: rethrow");
        throw;
    }
}

internal sealed class DemoResource : IDisposable
{
    private readonly string _name;
    private bool _disposed;

    public DemoResource(string name)
    {
        _name = name;
        Console.WriteLine($"open: {_name}");
    }

    public void Use()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Console.WriteLine($"use: {_name}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Console.WriteLine($"dispose: {_name}");
    }
}
