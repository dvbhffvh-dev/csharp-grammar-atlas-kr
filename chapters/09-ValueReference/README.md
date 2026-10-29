# 09. 값 형식, 참조 형식, ref

[이전: 08 메서드](../08-Methods/README.md) · [목차](../../README.md) · [다음: 10 클래스와 속성](../10-ClassesProperties/README.md)

## 시작하기

선수 지식: 변수, 배열, 대입, 메서드의 매개변수를 알고 있어야 합니다.
학습 목표는 대입할 때 무엇이 복사되는지 설명하고, 객체 변경과 변수 재대입을 구분하는 것입니다.
이 단원의 클래스와 구조체는 복사 동작을 관찰하기 위한 작은 예제입니다.
클래스의 설계 문법은 다음 단원에서 자세히 다룹니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/09-ValueReference/Chapter09.csproj
dotnet run --project chapters/09-ValueReference/Chapter09.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 연습문제 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 형식의 분류와 전달 방식은 다른 질문입니다

| 질문 | 선택지 | 결정하는 것 |
|---|---|---|
| 변수의 형식은 무엇인가? | 값 형식 / 참조 형식 | 변수에 담긴 값의 의미 |
| 매개변수를 어떻게 전달하는가? | 일반 전달 / `ref` 등 | 호출자의 변수 자체를 대상으로 하는지 |

`int`, `bool`, `double`, `struct`, `enum`은 값 형식입니다.
`class`, 배열, `string`은 참조 형식입니다.
값 형식 변수의 대입에서는 그 값이 복사됩니다.
참조 형식 변수의 대입에서는 객체를 가리키는 참조 값이 복사됩니다.
따라서 참조 형식도 일반 매개변수로 넘기면 **참조 값 자체를 값으로 복사**합니다.
이것을 `ref` 매개변수와 같은 뜻으로 설명하면 안 됩니다.

메모리 위치로만 구분하는 설명도 피해야 합니다.
값 형식이 항상 스택에 있고 참조 형식이 항상 힙에 있다는 규칙은 아닙니다.
클래스 객체 내부의 `int` 필드처럼 값 형식이 객체 안에 포함될 수 있습니다.
실제 저장 위치는 실행 환경과 사용 방식의 영향을 받습니다.
우선 **복사 후 두 변수가 같은 객체를 공유하는가**를 기준으로 추론하세요.

## 2. 예제 A: 숫자와 구조체 복사

```csharp
int original = 10;
int copy = original;
copy = 20;
Console.WriteLine($"value: {original} / {copy}");

Position first = new() { X = 2 };
Position second = first;
second.X = 9;
Console.WriteLine($"struct: {first.X} / {second.X}");
```

첫 줄에서 `original`에 10을 저장합니다.
둘째 줄에서 현재 값 10을 별도 변수 `copy`로 복사합니다.
셋째 줄은 `copy`만 바꾸므로 `original`은 10입니다.
`Position`도 `struct`로 선언했으므로 `second.X`의 변경은 `first.X`에 영향을 주지 않습니다.
`new()`는 왼쪽 형식에서 생성할 형식을 추론하는 C# 9 문법입니다.

구조체 복사가 모든 내부 객체를 복제한다는 뜻은 아닙니다.
구조체가 참조 형식 필드를 가지면 그 필드의 참조가 복사됩니다.
그 경우 복사된 두 구조체가 내부 객체를 공유할 수 있습니다.
이를 이해하면 얕은 복사와 깊은 복사의 차이도 이해하기 쉽습니다.

## 3. 예제 B: 같은 객체 변경과 새 객체 대입

```csharp
Counter left = new() { Value = 3 };
Counter right = left;
right.Value = 8;
Console.WriteLine($"reference: {left.Value} / {right.Value}");
right = new Counter { Value = 99 };
Console.WriteLine($"replace: {left.Value} / {right.Value}");
```

첫 줄은 객체 하나를 만들고 `left`가 가리키게 합니다.
둘째 줄은 그 객체를 가리키는 참조를 `right`에 복사합니다.
셋째 줄의 점 연산자는 `right`가 가리키는 객체에 접근합니다.
객체가 하나이므로 `left.Value`도 8입니다.
다섯째 줄은 다른 객체를 만들고 `right`에 새 참조를 저장합니다.
`left`는 이전 객체를 계속 가리키므로 마지막 출력은 8과 99입니다.

객체를 바꾸는 문장인 `right.Value = 8`과 변수에 대입하는 문장인 `right = ...`를 구분하세요.
복사 생성자나 복제 메서드 없이 `right = left`만 쓰면 별도 객체는 만들어지지 않습니다.

## 4. 예제 C: string은 변경 불가능한 참조 형식

```csharp
string word = "cat";
string other = word;
other += "!";
Console.WriteLine($"string: {word} / {other}");
```

`word`와 `other`는 처음에는 같은 문자열 내용을 참조합니다.
`+=`는 기존 문자열 객체의 글자를 수정하지 않습니다.
문자열 결합 결과를 얻어서 `other` 변수에 다시 대입합니다.
따라서 `word`는 `cat`, `other`는 `cat!`입니다.
**변경 불가능성**은 객체의 상태에 관한 성질이고, **값 형식 여부**는 형식의 분류입니다.
문자열을 값 형식이라고 외우면 이후 배열이나 객체 전달을 잘못 이해하게 됩니다.

## 5. 예제 D: ref는 호출자의 변수에 접근합니다

```csharp
AddOne(ref original);
Console.WriteLine($"ref: {original}");

static void AddOne(ref int value)
{
    value++;
}
```

호출부와 선언부 양쪽에 `ref`가 필요합니다.
호출 전에 `original`은 이미 초기화되어 있어야 합니다.
매개변수 `value`는 별도 숫자 복사본이 아니라 호출자의 변수를 참조합니다.
그래서 원래 10이었던 `original`이 11이 됩니다.
`ref`를 없애면 일반 값 전달이 되어 호출자의 숫자는 그대로입니다.
참조 형식에도 `ref`를 사용할 수 있으며, 그때는 호출자의 참조 변수 자체를 바꿀 수 있습니다.

## 6. 전체 기본 출력

```text
value: 10 / 20
struct: 2 / 9
reference: 8 / 8
replace: 8 / 99
string: cat / cat!
ref: 11
enum: Paid / 1
```

위 출력의 각 쌍을 보면서 '두 변수인가, 두 객체인가'를 따로 세어 보세요.
기본 예제는 항상 같은 결과를 냅니다.

## 7. 이름으로 상태를 표현하는 enum

`enum`은 관련된 정수 상수에 이름을 붙이는 값 형식입니다.
`Program.cs`의 `OrderState`에는 `Pending = 0`, `Paid = 1`, `Cancelled = 2`를 정의했습니다.

```csharp
OrderState state = OrderState.Paid;
Console.WriteLine($"enum: {state} / {(int)state}");
```

이름으로 출력하면 `Paid`, 정수로 명시적 변환하면 `1`이 나옵니다.
다른 사람이 코드를 읽을 때 의미를 알기 어려운 숫자 1 대신 `OrderState.Paid`를 사용할 수 있습니다.
기본 기반 형식은 `int`이며, 값을 생략하면 첫 값 0부터 1씩 증가합니다.
`default(OrderState)`는 0이므로, 0에 의미 있는 이름을 주는 설계가 유용합니다.
명시적 변환 `(OrderState)99`는 정의에 없는 숫자도 만들 수 있습니다.
외부에서 받은 숫자가 유효한 이름인지 확인하려면 `Enum.IsDefined` 등을 사용하세요.
비트 조합을 표현할 때는 `[Flags]`와 1, 2, 4처럼 겹치지 않는 비트 값을 사용합니다.
`[Flags]`가 임의의 숫자를 자동 검증하거나 값들을 자동으로 2의 거듭제곱으로 바꾸지는 않습니다.
여기서 사용하는 주문 상태는 서로 배타적인 하나의 상태이므로 `[Flags]`를 붙이지 않습니다.

## 8. 자주 하는 실수

| 실수 | 이유와 수정 방법 |
|---|---|
| 배열을 대입해서 백업했다고 생각함 | 배열은 참조 형식이므로 새 배열을 만들거나 복사 API를 사용합니다. |
| 문자열이 독립적으로 바뀌므로 값 형식이라고 생각함 | 문자열 객체는 불변이고 변수만 새 문자열로 바뀝니다. |
| 객체를 매개변수에 넘기면 무조건 `ref`라고 설명함 | 기본 전달은 참조 값의 복사이며 호출자의 변수 자체는 바뀌지 않습니다. |
| 구조체를 복사하면 내부 리스트도 복제된다고 생각함 | 참조 형식 필드의 참조도 그대로 복사됩니다. |

## 9. 연습문제

### 문제 1 · 기초: 배열 복사

`{ 1, 2 }` 배열의 독립적인 복사본을 만들고 복사본의 첫 값을 9로 바꾸세요.
원본의 첫 값은 1이어야 합니다.

<details>
<summary>해설과 확인할 결과</summary>

[해답 파일](ExerciseSolutions.cs)의 1번은 `Clone()`으로 별도 배열을 만듭니다.
반환 형식이 `object`이므로 `int[]`로 변환합니다.
배열의 원소가 `int`여서 이 문제에서는 원소까지 서로 독립적입니다.
참조 형식 원소를 담은 배열이라면 객체까지 깊게 복제되지는 않습니다.
예상 출력은 `1: 1 / 9`입니다.

</details>

### 문제 2 · 응용: 객체 변경 후 매개변수 재대입

`Counter`를 일반 매개변수로 전달하세요.
메서드에서 `Value`를 12로 바꾼 다음 매개변수에 값 99인 새 객체를 대입하세요.
호출자가 최종적으로 보는 값은 무엇일까요?

<details>
<summary>해설과 확인할 결과</summary>

[해답 파일](ExerciseSolutions.cs)의 `ChangeCounter`를 확인하세요.
처음 변경은 공유 객체에 적용되지만 두 번째 대입은 매개변수에만 적용됩니다.
호출자의 변수는 이전 객체를 계속 가리킵니다.
예상 출력은 `2: 12`입니다.

</details>

### 문제 3 · 도전: 두 숫자 교환

4와 7을 담은 두 변수의 값을 메서드 하나로 교환하세요.

<details>
<summary>해설과 확인할 결과</summary>

[해답 파일](ExerciseSolutions.cs)의 `Swap`은 두 매개변수에 `ref`를 사용합니다.
임시 변수에 첫 값을 저장해야 덮어쓴 원래 값을 잃지 않습니다.
호출 후 두 변수는 7과 4가 되어 `3: 7 / 4`를 출력합니다.

</details>

## 10. 복습 질문

1. `class` 변수 두 개를 대입하면 객체가 두 개가 되나요? → 아니요. 참조만 복사합니다.
2. `string`은 값 형식인가요? → 아니요. 변경 불가능한 참조 형식입니다.
3. 일반 `int` 매개변수와 `ref int`의 차이는? → 복사본을 바꾸는지 호출자 변수를 바꾸는지의 차이입니다.

## 공식 문서

- [Microsoft Learn: C# 형식 시스템](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/)
- [Microsoft Learn: 메서드 매개변수](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters)
