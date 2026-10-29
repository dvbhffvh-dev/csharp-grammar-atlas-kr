# 16. nullable 값 형식과 참조 형식

[이전: 15 예외와 자원](../15-ExceptionsResources/README.md) · [목차](../../README.md) · [다음: 17 델리게이트와 람다](../17-DelegatesLambdas/README.md)

## 시작하기

선수 지식: 값/참조 형식, 조건문, 메서드, 예외 처리입니다.
학습 목표는 값이 없음을 명시하고 null을 검사한 후 안전하게 사용하는 것입니다.
이 저장소는 nullable 분석을 켜고 컴파일 경고를 오류로 처리합니다.
따라서 null 경고를 무시하는 코드보다 의미 있는 검사와 기본값을 작성하는 습관을 익힙니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/16-Nullable/Chapter16.csproj
dotnet run --project chapters/16-Nullable/Chapter16.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 값이 없다는 상태를 어떻게 표현할까요?

0점은 측정된 점수이고 '점수 없음'은 아직 점수가 없다는 상태입니다.
빈 문자열은 길이가 0인 문자열이며 null과 다릅니다.
null은 참조할 대상이나 nullable 값이 없음을 나타냅니다.
의미가 다른 상태를 모두 0이나 빈 문자열로 합치면 데이터 해석이 어려워질 수 있습니다.

| 문법 | 의미 | 실행 시 형식 |
|---|---|---|
| `int` | 정수 값 | `System.Int32` |
| `int?` | 정수 또는 값 없음 | `System.Nullable<int>` |
| `string` | null이 아니어야 한다는 참조 계약 | `System.String` |
| `string?` | null일 수 있다는 참조 계약 | `System.String` |

값 형식의 `T?`와 참조 형식의 `T?`는 같은 표기지만 역할이 다릅니다.
`int?`는 실제로 값 존재 여부를 표현하는 별도 값 형식입니다.
`string?`는 새로운 런타임 클래스가 아니라 컴파일러 분석용 표기입니다.
C# 8에서 도입된 nullable 참조 분석은 실행 중 모든 null을 자동으로 차단하지 않습니다.

## 2. 예제 A: int?에서 값 꺼내기

```csharp
int? score = null;
Console.WriteLine($"value: {score.HasValue} / {score ?? -1}");
score = 80;
if (score is int actual)
{
    Console.WriteLine($"present: {actual}");
}
```

첫 줄은 아직 점수가 없다는 상태입니다.
`HasValue`는 값 존재 여부를 나타내므로 false입니다.
`score ?? -1`은 null이면 -1을 사용하고 값이 있으면 그 값을 사용합니다.
이는 원래 `score` 변수에 -1을 저장하는 대입이 아닙니다.
`score = 80` 뒤에는 실제 점수가 존재합니다.
`score is int actual`은 값이 있으면 검사와 값 꺼내기를 함께 수행합니다.
블록 안의 `actual`은 nullable이 아닌 `int`입니다.

`score.Value`로 값을 꺼낼 수도 있지만 null이면 예외가 발생합니다.
따라서 `HasValue` 확인이나 위 패턴을 사용하세요.
`GetValueOrDefault()`는 값이 없을 때 기본값 0을 돌려주므로 0과 값 없음의 구분을 잃을 수 있습니다.
기본값은 업무 의미에 맞게 선택해야 합니다.

## 3. 예제 B: ?.와 ??와 ??=

```csharp
string? name = null;
Console.WriteLine($"length: {name?.Length ?? 0}");
name ??= "Guest";
Console.WriteLine($"name: {name}");
```

`name?.Length`는 `name`이 null이면 멤버 접근을 건너뛰고 null을 반환합니다.
`Length`는 정수이므로 이 식의 결과는 `int?`입니다.
그 뒤 `?? 0`이 결과의 null을 기본 정수 0으로 바꿉니다.
따라서 첫 출력 길이는 0입니다.

`name ??= "Guest"`는 `name`이 null일 때만 실제로 값을 대입합니다.
이미 `"Min"` 같은 값이 있었다면 그 값을 유지합니다.
`??`는 결과를 선택하고 `??=`는 조건부로 변수에 저장한다는 차이가 있습니다.
오른쪽 식은 필요한 경우에만 평가되므로 단순 값 대신 함수 호출을 넣었을 때도 차이가 있습니다.
`??=`는 C# 8에서 도입된 문법입니다.

`?.`는 모든 예외를 무시하는 연산자가 아닙니다.
대상이 null일 때 접근을 생략할 뿐, 실행된 메서드 내부의 예외는 그대로 발생합니다.
null과 빈 문자열/공백을 함께 처리하려면 `IsNullOrWhiteSpace` 등 다른 의미 검사가 필요합니다.

## 4. 예제 C: 흐름 분석과 명시적 검사

```csharp
string? found = FindName(1);
if (found is not null)
{
    Console.WriteLine($"found: {found.ToUpperInvariant()}");
}

static string? FindName(int id) => id == 1 ? "min" : null;
```

반환 형식이 `string?`이므로 호출자는 결과가 없을 수 있음을 압니다.
`found is not null`을 통과한 블록에서는 값이 존재한다는 사실을 컴파일러가 추론합니다.
그 안에서는 별도의 `!` 없이 안전하게 메서드를 호출할 수 있습니다.
결과는 `MIN`입니다.
`is not` 패턴 문법은 C# 9에서 사용할 수 있습니다.

가능한 결과가 여러 개인 메서드는 계약에 null 가능성을 정확히 적어야 합니다.
무조건 기본 문자열을 돌려줄지, null을 돌려줄지, 예외를 던질지는 기능의 의미에 따라 결정합니다.
호출자에게 필요한 정보를 감추지 않는 것이 중요합니다.

## 5. 예제 D: !는 검사가 아닙니다

```csharp
string? missing = FindName(99);
string claimed = missing!;
Console.WriteLine($"forgiving still null: {claimed is null}");
```

`FindName(99)`는 null을 반환합니다.
뒤에 붙인 `!`는 null 허용 경고를 억제하는 연산자입니다.
실행 중 값을 바꾸거나 예외를 발생시켜 검증하지 않습니다.
따라서 `claimed`에는 실제 null이 들어 있으며 결과는 true입니다.
여기서는 이 사실을 보여 주기 위해 의도적으로 사용했고 멤버 접근은 하지 않습니다.

만약 다음 줄에서 `claimed.Length`를 호출하면 런타임 예외가 발생할 수 있습니다.
변수에 `string`이라고 적었다는 사실만으로 실제 값이 검증된 것은 아닙니다.
`!`를 경고를 지우는 기본 해결책으로 사용하지 마세요.
검사 후의 사실을 컴파일러가 알 수 없는 제한된 경우에만 근거를 가지고 사용해야 합니다.
접두어 `!condition`은 bool 논리 부정이며, 후위 `value!`와 역할이 다릅니다.

## 6. 함수 경계에서 계약을 검증하기

외부에서 null이 들어올 수 있는 경계에서는 실제 검사가 필요할 수 있습니다.
`ArgumentNullException.ThrowIfNull(value)`는 null이면 예외를 발생시킵니다.
검사 뒤에는 컴파일러도 해당 값이 null이 아님을 이해합니다.
직접 `if (value is null) throw ...`로 작성해도 같은 목적을 표현할 수 있습니다.
nullable 표기는 설계 의도를 전달하고 런타임 검사는 실제 값을 확인합니다.
두 방식의 역할을 구분해 함께 사용하세요.

## 7. 전체 기본 출력

```text
value: False / -1
present: 80
length: 0
name: Guest
found: MIN
forgiving still null: True
```

마지막 줄은 `!`가 null을 제거하지 않는다는 것을 실행으로 확인하는 부분입니다.
나머지 예제는 검사 또는 의미 있는 기본값으로 null을 처리합니다.

## 8. 자주 하는 실수

| 실수 | 이유와 해결 |
|---|---|
| null과 0을 같은 뜻으로 취급함 | 값 존재 여부와 실제 값을 분리합니다. |
| `int?`의 `Value`를 무조건 읽음 | 먼저 존재 여부를 검사합니다. |
| `?.`만 쓰면 결과도 항상 non-null이라고 생각함 | 결과도 null일 수 있으므로 필요한 후속 처리를 합니다. |
| 경고가 나오면 모두 `!`를 붙임 | null 가능성을 검토하고 검사나 기본값을 추가합니다. |
| `string?`이 별도의 런타임 클래스라고 생각함 | nullable 참조 표기는 컴파일러의 정적 분석 계약입니다. |

nullable 경고는 오류 가능성을 알려 주는 단서입니다.
경고를 없애기 위해 의미 없는 기본값을 넣으면 데이터 오류를 숨길 수 있습니다.
기본값, 실패 반환, 예외 중 호출자에게 가장 정확한 의미를 선택하세요.

## 9. 연습문제

### 문제 1 · 기초: 기본 수량

`int?` 수량이 null이면 1을 사용하되 실제 0이면 0을 유지하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번은 `quantity ?? 1`을 사용합니다.
`??`는 0을 false처럼 처리하지 않고 null만 검사합니다.
제공한 null 입력에서 예상 출력은 `1: 1`입니다.

</details>

### 문제 2 · 응용: 선택적인 문자열 길이

nullable 문자열을 Trim한 뒤 길이를 구하세요.
원래 문자열이 null이면 길이 0으로 처리하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 2번은 `text?.Trim().Length ?? 0`을 사용합니다.
null이면 이어진 접근이 단락 평가되어 길이를 읽지 않습니다.
`Trim()`은 non-null 문자열을 반환하므로 이 연속 접근은 안전합니다.
괄호로 체인을 끊는 경우에는 null 처리 범위가 달라질 수 있으니 주의하세요.
주어진 null 입력의 예상 출력은 `2: 0`입니다.

</details>

### 문제 3 · 도전: 필수 이름 검증

nullable 이름을 받아 null이면 예외를 던지고, 값이 있으면 Trim 후 대문자로 반환하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `NormalizeName`은 먼저 `ThrowIfNull`을 호출합니다.
그 이후에는 `!` 없이 일반 문자열 메서드를 호출할 수 있습니다.
입력 `"  min  "`은 `3: MIN`을 출력합니다.
null 입력은 바깥에서 처리하여 `3: name required`를 출력합니다.
공백만 있는 이름까지 금지하려면 null 검사와 별도로 내용 검증을 추가해야 합니다.

</details>

## 10. 복습 질문

1. `int?`와 `string?`의 중요한 차이는? → 전자는 별도 값 형식, 후자는 참조에 대한 컴파일러 분석용 표기입니다.
2. `??`와 `??=`의 차이는? → 전자는 결과를 선택하고 후자는 null인 대상에 실제 대입합니다.
3. 후위 `!`가 런타임 null을 검증하나요? → 아니요. 경고를 억제할 뿐 값은 그대로입니다.

## 공식 문서

- [Microsoft Learn: nullable 값 형식](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types)
- [Microsoft Learn: nullable 참조 형식](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/null-safety/nullable-reference-types)
