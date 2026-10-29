# 10. 클래스, 생성자, 필드와 속성

[이전: 09 값과 참조](../09-ValueReference/README.md) · [목차](../../README.md) · [다음: 11 상속과 다형성](../11-InheritancePolymorphism/README.md)

## 시작하기

선수 지식: 08단원의 메서드와 09단원의 참조 형식입니다.
학습 목표는 객체의 상태를 보호하면서 필요한 동작만 외부에 공개하는 것입니다.
예제는 입금만 가능한 아주 작은 학습용 계좌입니다.
금액에는 계산 흐름을 단순하게 보여 주기 위해 작은 정수를 사용합니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/10-ClassesProperties/Chapter10.csproj
dotnet run --project chapters/10-ClassesProperties/Chapter10.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 클래스와 객체

클래스는 어떤 데이터와 동작을 묶을지 정의하는 형식입니다.
객체는 그 형식으로 실제 생성한 대상입니다.
`Account`가 형식이라면 `new Account(...)`의 결과는 객체입니다.
같은 클래스로 여러 객체를 만들면 각 객체의 인스턴스 필드는 따로 존재합니다.
필드는 데이터를 저장하고 메서드는 작업을 수행합니다.
속성은 읽기와 쓰기를 위한 공개된 접근 지점입니다.

외부 코드가 필드를 마음대로 바꾸면 객체가 지켜야 할 규칙을 우회할 수 있습니다.
잔액을 직접 수정하게 두지 않고 `Deposit`으로만 바꾸게 하는 이유가 여기에 있습니다.
데이터와 관련 작업을 묶고 접근을 제한하는 설계를 캡슐화라고 합니다.
모든 필드를 숨기는 것만으로 좋은 설계가 되는 것은 아니며, 필요한 동작을 이해하기 쉽게 제공해야 합니다.

## 2. 접근 제한자

| 문법 | 접근 가능한 범위 |
|---|---|
| `public` | 해당 형식에 접근 가능한 모든 호출 코드 |
| `private` | 선언한 형식 내부 |
| `protected` | 선언한 클래스와 파생 클래스 |
| `internal` | 같은 어셈블리 |
| `protected internal` | 같은 어셈블리 **또는** 파생 클래스 |
| `private protected` | 같은 어셈블리에 있는 파생 클래스와 선언한 클래스 |

이 프로젝트의 최상위 클래스는 `internal`입니다.
클래스의 멤버에 접근 제한자를 생략하면 기본적으로 `private`입니다.
최상위 형식에 생략한 접근 제한자는 기본적으로 `internal`입니다.
따라서 'C#의 기본 접근성은 언제나 private'이라고 외우지 마세요.

## 3. 예제 A: 생성자와 초기화

```csharp
Account account = new("A-01", 100) { Owner = "Min" };
Console.WriteLine($"account: {account.Number} / {account.Owner} / {account.Balance}");
```

`new(...)`는 왼쪽의 `Account`에서 생성할 형식을 추론합니다.
먼저 생성자가 실행되어 번호와 초기 잔액을 설정합니다.
그다음 객체 초기화자의 `Owner = "Min"`이 실행됩니다.
마지막으로 완성된 객체의 참조가 `account`에 저장됩니다.

```csharp
public Account(string number, int openingBalance)
{
    ArgumentOutOfRangeException.ThrowIfNegative(openingBalance);
    _number = number;
    _balance = openingBalance;
    Created++;
}
```

생성자 이름은 형식 이름과 같고 반환 형식을 쓰지 않습니다.
첫 줄은 음수 잔액을 거부합니다. 예외 처리 문법은 15단원에서 다룹니다.
`_number`와 `_balance`는 객체마다 따로 저장됩니다.
`Created++`는 지금까지 이 코드 경로로 생성한 계좌 수를 늘립니다.
이름 앞의 밑줄은 필드를 구분하는 관례이며 문법상 필수는 아닙니다.

## 4. 예제 B: 필드를 숨기고 읽기만 허용하기

```csharp
private readonly string _number;
private int _balance;
public required string Owner { get; init; }
public string Number => _number;
public int Balance => _balance;
```

`Number`와 `Balance`는 계산식으로 값을 반환하는 읽기 전용 속성입니다.
외부에서 `account.Balance = 999;`로 직접 대입할 수 없습니다.
`readonly` 필드는 선언 또는 해당 생성자에서 초기화할 수 있고 이후 재대입할 수 없습니다.
참조 형식의 `readonly`는 참조의 재대입을 제한하며 내부 객체 전체를 불변으로 만들지는 않습니다.

`init` 접근자는 C# 9에서 도입되었습니다.
일반 `set`과 달리 객체 초기화 단계에서만 외부 대입을 허용합니다.
`required`는 C# 11에서 도입되었으며 호출자가 해당 멤버를 초기화하도록 요구합니다.
이 예제에서는 `Owner`를 생략한 객체 생성이 컴파일 오류가 됩니다.
하지만 `required`는 빈 문자열이나 공백을 거부하는 내용 검증이 아닙니다.
nullable 경고와도 별개인 초기화 계약이라는 점을 기억하세요.

자동 구현 속성 `{ get; set; }`에는 컴파일러가 저장 필드를 만들어 줍니다.
`{ get; private set; }`으로 만들면 읽기는 외부에 허용하고 쓰기는 형식 내부로 제한할 수 있습니다.
현재 예제의 `Created`가 그 형태입니다.

## 5. 예제 C: 메서드로 변경 규칙 유지하기

```csharp
public bool Deposit(int amount)
{
    if (amount <= 0)
    {
        return false;
    }

    _balance += amount;
    return true;
}
```

`amount`가 0 이하이면 상태를 바꾸기 전에 실패를 반환합니다.
양수이면 현재 잔액에 더한 뒤 성공을 반환합니다.
호출자는 `bool` 결과로 작업 성공 여부를 알 수 있습니다.
`Deposit(50)` 이후 잔액은 100에서 150으로 바뀝니다.
그다음 `Deposit(-10)`은 실패하므로 잔액은 계속 150입니다.
예제는 작은 정수만 사용하며, 일반화할 때는 정수 오버플로와 업무 규칙도 함께 설계해야 합니다.

## 6. 인스턴스, static, const, readonly 비교

| 종류 | 예제 | 특징 |
|---|---|---|
| 인스턴스 필드 | `_balance` | 객체별 상태 |
| 정적 속성 | `Created` | 이 형식에서 공유하는 상태 |
| 상수 | `Currency` | 컴파일 시 결정되는 값, 형식 이름으로 접근 |
| 읽기 전용 필드 | `_number` | 생성 때 정하고 재대입 금지 |

`public const string Currency = "KRW";`는 컴파일 시간 상수입니다.
`const` 멤버는 암시적으로 정적이므로 `static const`라고 쓰지 않습니다.
`readonly`는 실행 중 생성자 인수로 받은 값도 저장할 수 있습니다.
`static`은 '절대 바뀌지 않는다'가 아니라 '객체 하나에 속하지 않는다'는 뜻입니다.
따라서 `Created`는 정적이면서도 생성자에서 값이 바뀝니다.

## 7. 전체 기본 출력

```text
account: A-01 / Min / 100
deposit: True / 150
negative: False / 150
shared: 1 / KRW
```

`Created`는 이 실행에서 계좌를 하나 만들었으므로 1입니다.
프로그램을 새로 실행하면 정적 상태도 다시 초기화됩니다.

## 8. 자주 하는 실수

| 실수 | 해결 방향 |
|---|---|
| 생성자에 `void`를 붙임 | 생성자는 반환 형식을 쓰지 않습니다. |
| 모든 필드를 `public`으로 선언함 | 지켜야 할 규칙을 메서드와 속성에 모읍니다. |
| `init` 속성을 나중에 수정함 | 생성 시 지정하거나 변경 가능한 설계인지 검토합니다. |
| `required`면 내용도 유효하다고 생각함 | 공백, 범위 등의 검증은 별도로 구현합니다. |
| `readonly`면 리스트 원소도 바꿀 수 없다고 생각함 | 참조 재대입 제한과 객체 상태 변경은 구분합니다. |

## 9. 연습문제

### 문제 1 · 기초: 다른 객체 생성

번호 B-02, 초기 잔액 20, 소유자 Jin인 계좌를 만들고 이름과 잔액을 출력하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번처럼 생성자 인수와 객체 초기화자를 함께 씁니다.
번호와 잔액은 생성자가 받고 `required` 소유자 속성은 초기화자에서 지정합니다.
예상 출력은 `1: Jin / 20`입니다.

</details>

### 문제 2 · 응용: 실패해도 상태 유지

1번의 계좌에 0을 입금하고 성공 여부와 잔액을 출력하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 2번은 `Deposit(0)`을 호출합니다.
조건 검사에서 바로 반환하므로 잔액 변경 문장에 도달하지 않습니다.
예상 출력은 `2: False / 20`입니다.

</details>

### 문제 3 · 도전: 이름 변경 메서드 설계

책 제목을 읽을 수 있지만 직접 대입할 수는 없도록 만드세요.
생성자로 제목을 받고 `Rename`은 공백이 아닌 제목만 받아들이게 만드세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `Book`은 `Title { get; private set; }`을 사용합니다.
`Rename`에서 `IsNullOrWhiteSpace`를 검사한 뒤 대입합니다.
속성의 공개 범위와 변경 규칙을 분리하는 연습입니다.
해답 실행 결과는 `3: C# Guide`입니다.

</details>

## 10. 복습 질문

1. 생성자는 언제 실행되나요? → 객체를 생성할 때 실행되어 초기 상태를 만듭니다.
2. `static`과 `const`는 같은 뜻인가요? → 아니요. 공유 여부와 컴파일 시간 상수 여부는 다른 개념입니다.
3. 속성 대신 공개 필드만 쓰면 어떤 문제가 있나요? → 외부 변경을 검증하거나 제한하기 어려워집니다.

## 공식 문서

- [Microsoft Learn: 객체 지향 개요](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/)
- [Microsoft Learn: 속성](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties)
