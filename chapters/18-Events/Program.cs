if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

TemperatureSensor sensor = new();
EventHandler<TemperatureChangedEventArgs> display =
    (_, change) => Console.WriteLine($"temperature={change.OldValue}->{change.NewValue}");
EventHandler<TemperatureChangedEventArgs> alert = (_, change) =>
{
    if (change.NewValue >= 25)
    {
        Console.WriteLine($"alert={change.NewValue}");
    }
};

sensor.TemperatureChanged += display;
sensor.TemperatureChanged += alert;
sensor.SetTemperature(22);
sensor.SetTemperature(25);
sensor.SetTemperature(25); // 같은 값이면 이벤트를 발생시키지 않는다.

sensor.TemperatureChanged -= alert;
sensor.SetTemperature(30);
sensor.TemperatureChanged -= display;
sensor.SetTemperature(35); // 구독자가 없어도 안전하다.
Console.WriteLine($"final={sensor.Temperature}");

public sealed class TemperatureChangedEventArgs(int oldValue, int newValue) : EventArgs
{
    public int OldValue { get; } = oldValue;
    public int NewValue { get; } = newValue;
}

public sealed class TemperatureSensor
{
    public int Temperature { get; private set; }
    public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

    public void SetTemperature(int value)
    {
        if (Temperature == value)
        {
            return;
        }

        int oldValue = Temperature;
        Temperature = value;
        TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(oldValue, value));
    }
}
