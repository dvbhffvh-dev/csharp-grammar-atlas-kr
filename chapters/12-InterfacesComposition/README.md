# 12. 인터페이스와 객체 합성

[이전: 11 상속과 다형성](../11-InheritancePolymorphism/README.md) · [목차](../../README.md) · [다음: 13 제네릭](../13-Generics/README.md)

## 시작하기

선수 지식: 클래스, 생성자, 메서드와 11단원의 다형성입니다.
학습 목표는 구체 클래스 대신 작은 계약에 의존하고, 객체를 조합해 동작을 구성하는 것입니다.
문자열 서식을 적용하는 서비스를 통해 구현 교체 과정을 확인합니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/12-InterfacesComposition/Chapter12.csproj
dotnet run --project chapters/12-InterfacesComposition/Chapter12.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 인터페이스는 사용자가 기대할 수 있는 계약입니다

`interface`는 형식이 제공할 기능을 정의합니다.
이 단원의 `IFormatter`는 문자열 하나를 받아 문자열 하나를 돌려주는 계약입니다.
호출자는 내부 처리 방식 대신 계약에 선언된 메서드를 사용합니다.
구현 클래스는 해당 계약을 실제로 수행하는 코드를 제공합니다.

클래스는 기반 클래스를 하나만 상속할 수 있지만 인터페이스는 여러 개 구현할 수 있습니다.
서로 상속 관계가 없는 클래스도 같은 인터페이스를 구현할 수 있습니다.
구조체도 인터페이스를 구현할 수 있습니다.
이름 앞에 `I`를 붙이는 것은 C# 생태계의 일반적인 명명 관례입니다.

인터페이스를 곧바로 `new IFormatter()`로 생성할 수는 없습니다.
하지만 `IFormatter` 형식의 변수는 계약을 구현한 객체를 가리킬 수 있습니다.
계약은 문법만으로 의미까지 보장하지 않으므로 문서와 구현도 중요합니다.
예를 들어 '문자열을 반환한다'만으로 대문자 변환 여부까지 알 수는 없습니다.

## 2. 예제 A: 계약과 두 구현

```csharp
internal interface IFormatter
{
    string Format(string text);
}

internal sealed class UpperFormatter : IFormatter
{
    public string Format(string text) => text.ToUpperInvariant();
}

internal sealed class BracketFormatter : IFormatter
{
    public string Format(string text) => $"[{text}]";
}
```

인터페이스의 `Format` 선언은 매개변수와 반환 형식을 지정합니다.
일반적인 암시적 구현에서는 클래스의 구현 메서드를 `public`으로 선언합니다.
`UpperFormatter`는 지역 설정의 영향을 받지 않는 대문자 변환을 사용합니다.
`BracketFormatter`는 입력 양쪽에 대괄호를 붙입니다.
두 클래스는 처리 방식이 달라도 `IFormatter`라는 같은 계약을 만족합니다.

```csharp
IFormatter formatter = new UpperFormatter();
Console.WriteLine($"format: {formatter.Format("hello")}");
```

변수는 구현의 상세 멤버 대신 인터페이스의 멤버만 보게 됩니다.
실제 객체의 `Format`이 호출되어 `HELLO`가 반환됩니다.
이는 클래스 상속 없이도 사용할 수 있는 인터페이스 다형성입니다.

## 3. 예제 B: 서비스를 객체로 조합하기

```csharp
internal sealed class ReportService
{
    private readonly IFormatter _formatter;

    public ReportService(IFormatter formatter)
    {
        _formatter = formatter;
    }

    public string Create(string title) => _formatter.Format(title);
}
```

`ReportService`는 `IFormatter`를 구현하거나 상속하지 않습니다.
대신 서식 기능을 제공하는 객체를 필드로 보관합니다.
이처럼 다른 객체를 내부에 가지고 기능을 만드는 방식을 합성이라고 합니다.
`ReportService`는 서식기 **이다**가 아니라 서식기를 **가지고 있다**에 가깝습니다.

생성자 인수로 필요한 객체를 전달하는 방식은 생성자 주입이라고 합니다.
별도의 프레임워크나 자동 등록 기능 없이도 사용할 수 있습니다.
`readonly` 덕분에 생성 이후 서비스가 다른 서식기 참조로 교체하지 못합니다.
필드의 선언 형식이 인터페이스이므로 특정 구현 이름에 묶이지 않습니다.

```csharp
ReportService first = new(new UpperFormatter());
ReportService second = new(new BracketFormatter());
Console.WriteLine($"report: {first.Create("study")}");
Console.WriteLine($"report: {second.Create("study")}");
```

첫 서비스는 `STUDY`를 만들고 둘째 서비스는 `[study]`를 만듭니다.
서비스 코드는 동일하며 생성 때 전달한 협력 객체만 다릅니다.
새 구현을 추가해도 `Create`의 본문을 바꿀 필요가 없습니다.
실제 외부 입력을 받는 생성자라면 필요에 따라 null 검증도 추가할 수 있습니다.
이 예제의 모든 호출은 non-null 구현 객체를 전달합니다.

## 4. 예제 C: 여러 구현을 동일하게 사용하기

```csharp
IFormatter[] formatters = { new UpperFormatter(), new BracketFormatter() };
foreach (IFormatter item in formatters)
{
    Console.WriteLine($"loop: {item.Format("csharp")}");
}
```

배열은 인터페이스 참조를 담고 각 원소는 서로 다른 구현을 가리킵니다.
반복문 안에서는 메서드 이름, 인수, 반환 형식이 같으므로 공통 코드를 사용합니다.
첫 결과는 `CSHARP`, 둘째 결과는 `[csharp]`입니다.
구현에 따라 분기하는 코드를 호출자에게 계속 추가할 필요가 없습니다.

## 5. 추상 클래스와의 비교

| 판단 기준 | 추상 클래스 | 인터페이스 |
|---|---|---|
| 공통 인스턴스 상태와 생성 과정 | 필드와 생성자로 공유 가능 | 구현 객체가 자신의 상태를 관리 |
| 여러 계약 구현 | 클래스 상속은 하나 | 여러 인터페이스 구현 가능 |
| 관계의 중심 | 공통 기반 형식 | 제공할 수 있는 기능 |
| 이 단원의 용도 | 해당 없음 | 서식 처리 계약 |

현대 C# 인터페이스가 무조건 '본문 없는 메서드만' 담는 것은 아닙니다.
C# 8부터 기본 인터페이스 구현이 가능하고 이후 정적 추상 멤버 등도 추가되었습니다.
다만 일반 클래스처럼 인스턴스 필드를 보관하는 설계는 아닙니다.
첫 학습에서는 작은 인스턴스 계약과 일반 구현에 집중하면 충분합니다.
명시적 인터페이스 구현을 사용하면 해당 멤버를 인터페이스를 통해서만 호출하도록 만들 수도 있습니다.

## 6. 전체 기본 출력

```text
format: HELLO
report: STUDY
report: [study]
loop: CSHARP
loop: [csharp]
```

어떤 줄이 같은 서비스의 다른 구성이고, 어떤 줄이 직접 인터페이스를 호출한 것인지 구분해 보세요.

## 7. 자주 하는 실수

| 실수 | 이유와 해결 |
|---|---|
| `new IFormatter()`로 객체를 생성함 | 인터페이스를 구현한 구체 클래스를 생성합니다. |
| 암시적 구현 메서드에서 `public`을 생략함 | 계약을 충족하도록 공개 메서드를 선언합니다. |
| 생성자 안에서 특정 구현을 항상 직접 생성함 | 교체할 협력 객체는 생성자 매개변수로 전달할 수 있습니다. |
| 합성을 상속과 같은 뜻으로 이해함 | 합성은 다른 객체를 보유하고 그 객체에 작업을 위임합니다. |
| 메서드를 하나 추가할 때마다 거대한 인터페이스를 만듦 | 사용자의 역할에 맞는 작은 계약으로 나눌 수 있는지 생각합니다. |

인터페이스는 모든 클래스에 기계적으로 붙여야 하는 문법은 아닙니다.
교체 가능한 역할이나 경계를 표현할 때 사용하면 목적이 분명합니다.

## 8. 연습문제

### 문제 1 · 기초: 접두어 구현

`title: `를 입력 앞에 붙이는 `PrefixFormatter`를 만드세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `PrefixFormatter`는 `IFormatter`를 구현합니다.
`Format`의 반환 형식과 매개변수를 계약과 같게 선언합니다.
`note`를 넘기면 `1: title: note`가 출력됩니다.

</details>

### 문제 2 · 응용: 서비스에 새 구현 주입

1번 구현을 기존 `ReportService`에 전달해 `plan`을 처리하세요.
서비스의 코드는 수정하지 마세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 2번은 기존 생성자에 새 구현을 전달합니다.
서비스는 구체 이름 대신 인터페이스만 알면 되므로 그대로 동작합니다.
예상 출력은 `2: title: plan`입니다.

</details>

### 문제 3 · 도전: 두 처리 단계 합성

두 `IFormatter`를 받아 첫 결과를 둘째 입력으로 넘기는 `DoubleFormatter`를 만드세요.
대문자 처리 후 대괄호 처리를 적용하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `DoubleFormatter`는 스스로도 `IFormatter`입니다.
내부의 두 구현을 필드에 보관하고 `_second.Format(_first.Format(text))`로 위임합니다.
구성 순서가 결과에 영향을 줄 수 있으므로 처리 순서를 계약에 명시해야 합니다.
입력 `book`에 대한 예상 출력은 `3: [BOOK]`입니다.

</details>

## 9. 복습 질문

1. 인터페이스 변수에 무엇을 담을 수 있나요? → 그 계약을 구현한 객체의 참조를 담을 수 있습니다.
2. 생성자 주입에 별도 라이브러리가 필요한가요? → 아니요. 일반 생성자 매개변수만으로 가능합니다.
3. 합성의 핵심은 무엇인가요? → 필요한 기능의 객체를 보유하고 그 객체에 작업을 위임하는 것입니다.

## 공식 문서

- [Microsoft Learn: interface](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface)
- [Microsoft Learn: 인터페이스 개요](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces)
