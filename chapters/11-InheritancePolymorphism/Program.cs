if (args.Contains("--answers"))
{
    ExerciseSolutions.Run();
    return;
}

// 변수의 선언 형식은 Shape, 실제 객체의 형식은 Rectangle입니다.
Shape shape = new Rectangle(3, 4);
Console.WriteLine($"area: {shape.Area()}");
Console.WriteLine($"describe: {shape.Describe()}");

Shape[] shapes = { new Rectangle(2, 5), new Square(3) };
foreach (Shape item in shapes)
{
    Console.WriteLine($"loop: {item.Area()}");
}

// new는 가상 재정의가 아니라 이름 숨김입니다.
BaseLabel baseView = new FancyLabel();
FancyLabel derivedView = new();
Console.WriteLine($"hidden: {baseView.Label()} / {derivedView.Label()}");

internal abstract class Shape
{
    public abstract int Area();
    public virtual string Describe() => "shape";
}

internal sealed class Rectangle : Shape
{
    private readonly int _width;
    private readonly int _height;

    public Rectangle(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public override int Area() => _width * _height;
    public override string Describe() => "rectangle";
}

internal sealed class Square : Shape
{
    private readonly int _side;

    public Square(int side)
    {
        _side = side;
    }

    public override int Area() => _side * _side;
}

internal class BaseLabel
{
    public string Label() => "base";
}

internal sealed class FancyLabel : BaseLabel
{
    public new string Label() => "fancy";
}
