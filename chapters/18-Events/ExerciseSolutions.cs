internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. 구독 여부와 무관하게 발행자는 자신의 상태를 변경한다.
        TemperatureSensor quiet = new();
        quiet.SetTemperature(10);
        Console.WriteLine($"without-subscriber={quiet.Temperature}");

        // 2. 중복 값은 센서에서 걸러지므로 알림 횟수에 포함되지 않는다.
        TemperatureSensor sensor = new();
        int calls = 0;
        EventHandler<TemperatureChangedEventArgs> count = (_, _) => calls++;
        sensor.TemperatureChanged += count;
        sensor.SetTemperature(10);
        sensor.SetTemperature(10);
        sensor.SetTemperature(20);
        sensor.TemperatureChanged -= count;
        sensor.SetTemperature(30);
        Console.WriteLine($"notifications={calls}");

        // 3. 리소스의 수명과 이벤트 구독 수명을 같이 관리한다.
        using (ThresholdListener listener = new(sensor, 40))
        {
            sensor.SetTemperature(40);
            sensor.SetTemperature(45);
        }
        sensor.SetTemperature(50); // listener는 이미 해제되었다.
    }

    private sealed class ThresholdListener : IDisposable
    {
        private readonly TemperatureSensor _sensor;
        private readonly int _threshold;
        private bool _disposed;

        public ThresholdListener(TemperatureSensor sensor, int threshold)
        {
            _sensor = sensor;
            _threshold = threshold;
            _sensor.TemperatureChanged += Handle;
        }

        private void Handle(object? sender, TemperatureChangedEventArgs change)
        {
            if (change.NewValue >= _threshold)
            {
                Console.WriteLine($"threshold={change.NewValue}");
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _sensor.TemperatureChanged -= Handle;
            _disposed = true;
        }
    }
}
