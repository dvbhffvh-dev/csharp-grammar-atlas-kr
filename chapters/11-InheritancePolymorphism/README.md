# 11. 상속, 추상 클래스, 다형성

[이전: 10 클래스와 속성](../10-ClassesProperties/README.md) · [목차](../../README.md) · [다음: 12 인터페이스와 합성](../12-InterfacesComposition/README.md)

## 시작하기

선수 지식: 클래스, 생성자, 접근 제한자, 메서드입니다.
학습 목표는 기반 형식으로 여러 구현을 사용하고 `override`와 `new`를 구분하는 것입니다.
도형의 넓이를 구하는 예제로 메서드가 선택되는 과정을 관찰합니다.
예제의 길이는 양의 작은 정수이며, 넓이 계산에만 집중합니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/11-InheritancePolymorphism/Chapter11.csproj
dotnet run --project chapters/11-InheritancePolymorphism/Chapter11.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 상속을 사용하는 이유

`class Rectangle : Shape`에서 `Shape`는 기반 클래스, `Rectangle`은 파생 클래스입니다.
파생 클래스는 기반 클래스가 제공하는 계약과 구현을 이어받습니다.
C# 클래스는 직접 기반 클래스를 하나만 가질 수 있습니다.
여러 기능 계약을 구현하려면 다음 단원의 인터페이스를 사용할 수 있습니다.

상속 관계에서는 파생 객체를 기반 형식이 필요한 자리에 전달할 수 있습니다.
사각형을 도형으로 다루는 것이 자연스러운 예입니다.
단순히 코드 몇 줄을 재사용하려고 관련 없는 형식끼리 상속시키면 변경이 어려워집니다.
'이 객체를 기반 형식 대신 사용해도 호출자의 기대가 유지되는가'를 먼저 생각하세요.

기반 클래스의 `private` 멤버는 파생 클래스 코드에서 직접 접근할 수 없습니다.
기반 생성자가 필요하면 파생 생성자의 `: base(...)`로 인수를 전달합니다.
생성자는 상속되는 일반 메서드가 아니며 객체 생성 과정에서 기반 생성자도 실행됩니다.

## 2. abstract, virtual, override, sealed

| 문법 | 의미 |
|---|---|
| `abstract class` | 직접 객체를 만들 수 없는 기반 형식 |
| `abstract` 메서드 | 본문 없이 계약만 정의하고 구체 파생 형식에 구현 요구 |
| `virtual` 메서드 | 기본 구현을 제공하되 재정의 허용 |
| `override` 메서드 | 상속받은 가상/추상 멤버를 재정의 |
| `sealed class` | 더 이상 파생 클래스를 만들 수 없음 |
| `sealed override` | 이번 재정의는 사용하되 추가 재정의 금지 |

추상 클래스에 반드시 추상 메서드가 있어야 하는 것은 아닙니다.
추상 메서드를 선언하는 클래스는 추상 클래스여야 합니다.
일반 비가상 메서드를 `override`할 수는 없습니다.
`override`를 사용할 때는 이름과 매개변수 등 재정의 계약을 맞춰야 합니다.

## 3. 예제 A: 기반 계약과 실제 구현

```csharp
internal abstract class Shape
{
    public abstract int Area();
    public virtual string Describe() => "shape";
}
```

`Area`는 모든 구체 도형이 제공해야 하는 연산입니다.
도형마다 계산법이 달라 기반 클래스에는 본문을 쓰지 않았습니다.
`Describe`는 기본 문자열을 반환하므로 파생 클래스가 그대로 사용해도 됩니다.

```csharp
Shape shape = new Rectangle(3, 4);
Console.WriteLine($"area: {shape.Area()}");
Console.WriteLine($"describe: {shape.Describe()}");
```

첫 줄의 변수 선언 형식은 `Shape`입니다.
실제로 만들어진 객체의 형식은 `Rectangle`입니다.
`Area()`는 가상 호출이므로 실제 객체의 재정의를 실행하여 12를 반환합니다.
`Describe()` 역시 `Rectangle`의 재정의를 실행하여 `rectangle`을 반환합니다.
이렇게 공통 형식으로 여러 구체 동작을 사용하는 성질을 다형성이라고 합니다.

## 4. 예제 B: 구현을 추가해도 반복 코드는 같습니다

```csharp
Shape[] shapes = { new Rectangle(2, 5), new Square(3) };
foreach (Shape item in shapes)
{
    Console.WriteLine($"loop: {item.Area()}");
}
```

배열의 원소 형식은 `Shape`이므로 두 파생 형식의 객체를 담을 수 있습니다.
첫 원소에서는 사각형의 넓이 2 × 5를 계산합니다.
둘째 원소에서는 정사각형의 넓이 3 × 3을 계산합니다.
반복문은 구체 형식 이름이나 별도의 `if` 분기를 알 필요가 없습니다.

```csharp
public override int Area() => _side * _side;
```

위 문장은 `Square`의 구현입니다.
`Square`는 `Describe`를 재정의하지 않았으므로 기반 구현인 `shape`를 그대로 사용합니다.
기반 형식 변수를 쓰더라도 모든 멤버가 자동으로 실제 형식에 따라 바뀌는 것은 아닙니다.
그 동작은 가상 멤버 재정의에 대한 규칙입니다.

## 5. 예제 C: new는 숨김이며 override가 아닙니다

```csharp
internal class BaseLabel
{
    public string Label() => "base";
}

internal sealed class FancyLabel : BaseLabel
{
    public new string Label() => "fancy";
}
```

기반 `Label`에는 `virtual`이 없습니다.
파생 `Label`의 `new`는 같은 이름을 의도적으로 숨긴다는 표시입니다.
여기서 `new`는 객체 생성 연산자가 아니라 멤버 선언의 한정자입니다.

```csharp
BaseLabel baseView = new FancyLabel();
FancyLabel derivedView = new();
Console.WriteLine($"hidden: {baseView.Label()} / {derivedView.Label()}");
```

일반적인 이 호출에서는 변수의 컴파일 시간 형식에 따라 숨겨진 멤버를 선택합니다.
`baseView`를 통해서는 `BaseLabel.Label`을 호출합니다.
`derivedView`를 통해서는 `FancyLabel.Label`을 호출합니다.
따라서 결과는 `base / fancy`입니다.
동적으로 실제 객체에 맞춘 재정의를 원했다면 기반에 `virtual`, 파생에 `override`를 사용해야 합니다.

## 6. 전체 기본 출력

```text
area: 12
describe: rectangle
loop: 10
loop: 9
hidden: base / fancy
```

처음 네 줄은 가상/추상 메서드의 재정의 결과입니다.
마지막 줄은 이름 숨김과 선언 형식의 영향을 보여 줍니다.

## 7. 자주 하는 실수

| 실수 | 원인과 해결 |
|---|---|
| `new Shape()`를 호출함 | 추상 클래스는 직접 생성할 수 없으므로 구체 형식을 생성합니다. |
| `new`로 숨긴 메서드가 다형적으로 호출될 거라고 생각함 | 가상 재정의를 원하면 `virtual`과 `override`를 사용합니다. |
| 비슷한 속성이 있다는 이유만으로 상속함 | 기반 형식의 역할을 정말 대신할 수 있는지 검토합니다. |
| 기반 형식 변수에서 파생 형식에만 있는 메서드를 호출함 | 선언 형식의 계약에 그 멤버가 없으면 직접 호출할 수 없습니다. |
| 파생 클래스에서 기반의 private 필드를 직접 사용함 | 기반이 제공하는 메서드나 적절한 접근 범위를 사용합니다. |

다운캐스트는 실제 형식이 맞을 때만 안전합니다.
공통 동작을 수행할 때 매번 구체 형식으로 캐스팅해야 한다면 계약을 다시 생각해 보세요.
형식 검사와 패턴 매칭은 뒤 단원에서 더 다룹니다.

## 8. 연습문제

### 문제 1 · 기초: 기본 가상 구현 확인

변 길이가 4인 `Square`를 `Shape` 변수에 담으세요.
넓이와 설명을 출력하고 두 메서드의 구현이 어디에 있는지 말해 보세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번처럼 기반 형식으로 호출합니다.
`Area`는 `Square` 재정의이고 `Describe`는 `Shape`의 기본 구현입니다.
예상 출력은 `1: 16 / shape`입니다.

</details>

### 문제 2 · 응용: 새로운 구체 도형

밑변 6, 높이 4인 직각삼각형을 구현하세요.
이 문제에서는 두 변의 곱이 짝수인 정수 입력만 사용합니다.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `RightTriangle : Shape`을 확인하세요.
넓이를 `_width * _height / 2`로 계산하고 `Area`를 재정의합니다.
일반적인 소수 넓이까지 지원하려면 계약의 반환 형식부터 바꿔야 합니다.
이 문제의 예상 출력은 `2: 12`입니다.

</details>

### 문제 3 · 도전: 넓이 합산

1번 정사각형, 2번 삼각형, 가로 2·세로 3 사각형의 넓이를 합산하세요.
합산 반복문에서 구체 도형 이름으로 분기하지 마세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 3번은 `Shape[]`를 순회합니다.
각 원소에 `Area()`를 호출하면 실제 객체의 구현이 선택됩니다.
16 + 12 + 6이므로 `3: 34`를 출력합니다.

</details>

## 9. 복습 질문

1. `abstract` 메서드와 `virtual` 메서드의 차이는? → 전자는 본문이 없고 후자는 기본 본문이 있습니다.
2. `Shape` 변수로 `Rectangle.Area`를 호출할 수 있는 이유는? → 가상 호출이 실제 객체의 재정의를 선택하기 때문입니다.
3. `new`로 숨기는 것과 `override`는 같은가요? → 아니요. 숨김과 가상 재정의는 멤버 선택 규칙이 다릅니다.

## 공식 문서

- [Microsoft Learn: 다형성](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/polymorphism)
- [Microsoft Learn: 상속](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/inheritance)
