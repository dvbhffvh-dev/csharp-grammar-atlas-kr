# 13. 제네릭 형식, 메서드, 제약 조건

[이전: 12 인터페이스와 합성](../12-InterfacesComposition/README.md) · [목차](../../README.md) · [다음: 14 컬렉션](../14-Collections/README.md)

## 시작하기

선수 지식: 클래스, 메서드, 인터페이스, 매개변수와 반환값입니다.
학습 목표는 자료형만 다른 중복 코드를 줄이고 필요한 연산을 제약 조건으로 표현하는 것입니다.
이 단원을 익히면 다음 단원의 `List<int>`, `Dictionary<string, int>`도 읽을 수 있습니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/13-Generics/Chapter13.csproj
dotnet run --project chapters/13-Generics/Chapter13.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. T는 값이 아니라 형식을 받는 자리입니다

일반 메서드 매개변수 `int count`는 실행에 사용할 값을 받습니다.
제네릭의 형식 매개변수 `T`는 그 코드에 사용할 자료형을 나타냅니다.
`Box<int>`에서 `int`는 형식 인수입니다.
`Box<string>`에서 `string`은 다른 형식 인수입니다.

숫자 보관함과 문자열 보관함을 별도 클래스로 복사할 필요 없이 `Box<T>` 하나로 표현할 수 있습니다.
컴파일러는 각 사용 지점에서 형식 관계를 검사합니다.
`Box<int>`의 `Value`를 읽으면 `int`이므로 `object`를 거쳐 캐스팅할 필요가 없습니다.
`T`라는 이름은 관례이며 `TKey`, `TValue`, `TItem` 같은 의미 있는 이름도 사용할 수 있습니다.

제네릭은 `dynamic`과 다릅니다.
형식 검사를 실행 시점까지 미루는 대신, 형식 매개변수와 제약을 통해 컴파일 시 검사합니다.
`var`도 제네릭 자체가 아니라 지역 변수 형식을 추론하는 문법입니다.
세 가지를 각각의 역할로 구분하세요.

## 2. 예제 A: 제네릭 클래스

```csharp
internal sealed class Box<T>
{
    public T Value { get; }

    public Box(T value)
    {
        Value = value;
    }
}
```

클래스 이름 뒤의 `<T>`가 형식 매개변수를 선언합니다.
속성의 형식과 생성자 인수 형식에 같은 `T`를 사용합니다.
호출자가 고른 형식을 처음부터 끝까지 유지하는 것이 핵심입니다.

```csharp
Box<int> number = new(42);
Box<string> word = new("atlas");
Console.WriteLine($"box: {number.Value} / {word.Value}");
```

첫 번째 객체는 정수만 보관하도록 만들어졌습니다.
두 번째 객체는 문자열을 보관합니다.
따라서 `new Box<int>("atlas")`는 컴파일할 수 없습니다.
`Box<int>`와 `Box<string>`은 서로 다른 구성된 형식이므로 서로 대입할 수 없습니다.

## 3. 예제 B: 제네릭 메서드와 형식 추론

```csharp
public static T Echo<T>(T value) => value;
```

메서드 이름 뒤에 `<T>`를 선언하면 메서드 자체가 제네릭입니다.
반드시 클래스까지 제네릭이어야 하는 것은 아닙니다.
이 메서드는 입력의 형식을 그대로 유지하여 반환합니다.

```csharp
Console.WriteLine($"echo: {GenericTools.Echo(7)} / {GenericTools.Echo("go")}");
```

첫 호출은 인수 7에서 `T = int`를 추론합니다.
둘째 호출은 인수 `"go"`에서 `T = string`을 추론합니다.
명시적으로 `Echo<int>(7)`이라고 써도 됩니다.
반환받을 변수 형식만으로 일반 메서드의 형식 인수를 항상 추론해 주지는 않습니다.
입력 인수가 없는 `Create<T>()`와 `Zero<T>()`에는 아래 예제처럼 형식 인수를 씁니다.

## 4. 예제 C: 제약 조건으로 가능한 연산을 알리기

```csharp
public static T Max<T>(T left, T right) where T : IComparable<T>
    => left.CompareTo(right) >= 0 ? left : right;
```

아무 제약 없는 `T`가 반드시 `CompareTo`를 가진다는 보장은 없습니다.
`where T : IComparable<T>`는 자기 형식과 비교할 수 있는 형식만 받겠다는 계약입니다.
컴파일러는 이 조건 덕분에 `left.CompareTo(right)` 호출을 허용합니다.
비교 결과가 양수면 왼쪽이 크고, 0이면 같고, 음수면 오른쪽이 큽니다.
`Max(4, 9)`는 9를 반환합니다.

제네릭 `T`에 `+`나 `>`를 무조건 사용할 수 있는 것은 아닙니다.
모든 형식이 해당 연산을 제공하는 것이 아니기 때문입니다.
숫자 전용 일반화를 위해서는 현대 .NET의 제네릭 수학 같은 별도 계약이 필요합니다.
이 단원에서는 비교 인터페이스를 통해 필요한 능력을 명시합니다.

| 흔한 제약 | 허용하거나 요구하는 것 |
|---|---|
| `where T : struct` | nullable이 아닌 값 형식 |
| `where T : class` | 참조 형식, nullable 분석에서는 비-null 계약 |
| `where T : notnull` | nullable이 아닌 값 또는 참조 형식 계약 |
| `where T : IComparable<T>` | 비교 인터페이스 구현 |
| `where T : BaseType` | 지정한 기반 클래스 또는 파생 클래스 |
| `where T : new()` | 공개 매개변수 없는 생성자로 생성 가능한 형식 |

여러 제약을 쓸 때는 정해진 조합과 순서 규칙을 지켜야 합니다.
`new()`는 다른 일반 제약 뒤에 씁니다.
`struct` 제약에는 생성 관련 조건이 포함되므로 `struct, new()`를 함께 쓰지 않습니다.
nullable 관련 제약 위반 중 일부는 경고이며, 이 저장소는 경고도 오류로 취급합니다.

## 5. 예제 D: default와 new는 다릅니다

```csharp
public static T Zero<T>() where T : struct => default;
public static T Create<T>() where T : new() => new T();
```

`default`는 형식의 기본값을 만듭니다.
`default(int)`는 0, `default(bool)`은 false입니다.
참조 형식의 기본값은 null이며, 새 객체를 생성하는 동작이 아닙니다.
여기서는 `struct` 제약을 사용하여 값 형식의 기본값만 반환합니다.
`default`의 이름을 `Zero`로 지었지만 모든 구조체에 수학적 0의 의미가 있는 것은 아닙니다.

`new T()`는 생성 과정을 수행합니다.
`new()` 제약 덕분에 호출 가능한 공개 매개변수 없는 생성자가 보장됩니다.
예제의 `Ticket`은 그런 생성자를 제공하고 `Name`을 `ticket`으로 초기화합니다.
사용자 정의 구조체는 매개변수 없는 생성자를 정의할 수 있으므로 `default(T)`와 `new T()`를 항상 같은 동작으로 생각하면 안 됩니다.

## 6. 전체 기본 출력

```text
box: 42 / atlas
echo: 7 / go
max: 9
default: 0 / False
new: ticket
```

`False`는 `bool` 출력의 기본 문자열 표현입니다.
문자열 최대값은 문화권별 비교 영향을 설명해야 하므로 기본 예제에서는 정수만 비교합니다.

## 7. 자주 하는 실수

| 실수 | 이유와 해결 |
|---|---|
| `T`를 문자열로 전달함 | `"int"`가 아니라 형식 인수 `<int>`를 사용합니다. |
| 제약 없이 `T`의 임의 메서드를 호출함 | 필요한 인터페이스나 기반 형식 제약을 선언합니다. |
| `default`가 생성자를 실행한다고 생각함 | 기본값과 생성은 다른 동작입니다. |
| `List<Derived>`를 `List<Base>`에 바로 대입함 | 제네릭 형식마다 변환 규칙이 다르며 `List<T>`는 그런 변환을 지원하지 않습니다. |
| 모든 메서드를 제네릭으로 만듦 | 실제로 형식만 달라지는 공통 로직일 때 사용합니다. |

## 8. 연습문제

### 문제 1 · 기초: 다른 형식 사용

`Box<bool>`에 true를 담고 값을 출력하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번은 형식 인수만 `bool`로 바꿉니다.
기존 클래스의 본문을 복사하거나 수정하지 않아도 됩니다.
예상 출력은 `1: True`입니다.

</details>

### 문제 2 · 응용: 제네릭 교환

`Swap<T>(ref T left, ref T right)`를 만들어 정수 2와 8을 교환하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `Swap`은 임시 변수도 `T`로 선언합니다.
값의 대입만 필요하므로 비교나 생성 관련 제약은 필요 없습니다.
호출 인수로 `T`가 추론되며 예상 출력은 `2: 8 / 2`입니다.

</details>

### 문제 3 · 도전: 배열 최대값

`IComparable<T>`를 만족하는 배열의 최대값을 반환하세요.
빈 배열은 허용하지 않으며, 음수만 있는 배열도 처리할 수 있어야 합니다.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `Greatest`는 첫 원소를 초기 최댓값으로 사용합니다.
0으로 시작하면 음수만 있는 배열을 잘못 처리하므로 첫 원소가 중요합니다.
둘째 원소부터 비교하여 더 큰 값을 저장합니다.
빈 배열은 별도로 검사해 명확한 예외를 발생시킵니다. 예외 문법은 15단원에서 다룹니다.
`{ 3, 9, 4 }`에 대한 예상 출력은 `3: 9`입니다.

</details>

## 9. 복습 질문

1. 형식 매개변수와 일반 매개변수의 차이는? → 형식 자체를 받는지 실제 값을 받는지의 차이입니다.
2. 제약 조건은 왜 필요한가요? → 제네릭 코드에서 사용할 수 있는 기능을 컴파일러와 호출자에게 알려 줍니다.
3. `default(string)`은 빈 문자열인가요? → 아니요. null입니다.

## 공식 문서

- [Microsoft Learn: 제네릭 형식과 메서드](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)
- [Microsoft Learn: where 제약 조건](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-generic-type-constraint)
