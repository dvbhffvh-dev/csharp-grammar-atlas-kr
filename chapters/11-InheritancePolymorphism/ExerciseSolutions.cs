internal static class ExerciseSolutions
{
    public static void Run()
    {
        // 1. override를 생략한 virtual 멤버는 기본 구현을 사용합니다.
        Shape square = new Square(4);
        Console.WriteLine($"1: {square.Area()} / {square.Describe()}");

        // 2. 호출부를 바꾸지 않고 새 구체 형식을 추가합니다.
        Shape triangle = new RightTriangle(6, 4);
        Console.WriteLine($"2: {triangle.Area()}");

        // 3. 배열의 각 원소에서 가상 호출을 수행합니다.
        Shape[] shapes = { square, triangle, new Rectangle(2, 3) };
        int total = 0;
        foreach (Shape shape in shapes)
        {
            total += shape.Area();
        }

        Console.WriteLine($"3: {total}");
    }

    private sealed class RightTriangle : Shape
    {
        private readonly int _width;
        private readonly int _height;

        public RightTriangle(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public override int Area() => _width * _height / 2;
    }
}
