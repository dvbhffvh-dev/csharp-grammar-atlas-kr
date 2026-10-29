# 15. 예외 처리와 자원 정리

[이전: 14 컬렉션](../14-Collections/README.md) · [목차](../../README.md) · [다음: 16 nullable](../16-Nullable/README.md)

## 시작하기

선수 지식: 메서드 호출, 조건문, 클래스, 인터페이스입니다.
학습 목표는 실패를 처리할 위치를 정하고 예외가 발생해도 자원을 정리하는 것입니다.
예제의 자원은 파일이나 네트워크가 아닌 출력용 모형 객체입니다.
따라서 외부 파일이나 사용자 입력 없이 매번 같은 순서를 관찰할 수 있습니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/15-ExceptionsResources/Chapter15.csproj
dotnet run --project chapters/15-ExceptionsResources/Chapter15.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 예외는 실패를 호출 경로 밖으로 전달합니다

메서드는 정상적으로 반환하거나 예외를 던져 정상 흐름을 중단할 수 있습니다.
예외가 발생하면 처리 가능한 `catch`를 호출 경로를 따라 찾습니다.
그 과정에서 벗어나는 범위의 정리 코드가 실행됩니다.
처리하지 않은 예외는 콘솔 프로그램을 종료시킬 수 있습니다.

예외를 단순히 '오류 메시지 문자열'로만 생각하지 마세요.
예외 객체는 형식, 메시지, 호출 경로 등의 정보를 담습니다.
형식을 통해 실패 종류를 구분하고 적절한 위치에서 처리합니다.
입력의 일반적인 실패까지 무조건 예외로 처리해야 하는 것은 아닙니다.
예를 들어 숫자로 바꿀 수 없는 사용자 입력은 `int.TryParse`로 처리하기 좋습니다.

## 2. try, catch, finally의 역할

| 문법 | 역할 |
|---|---|
| `try` | 예외가 발생할 수 있는 작업 범위 |
| `catch` | 특정 예외를 처리하는 코드 |
| `finally` | 범위를 벗어날 때 실행할 정리 코드 |
| `throw new ...` | 새로운 예외 생성 및 발생 |
| `throw;` | 현재 처리 중인 예외를 다시 전달 |

`catch`는 잡을 수 있는 더 구체적인 예외부터 작성합니다.
상위 `Exception`을 먼저 잡으면 뒤의 더 구체적인 처리에 도달하지 못합니다.
복구할 수 없는 상황까지 무조건 삼키면 실패가 성공처럼 보일 수 있습니다.
복구, 의미 있는 변환, 기록 후 재전달 등 잡는 목적을 분명히 하세요.

`finally`는 예외 유무와 상관없이 일반적인 제어 흐름에서 실행됩니다.
`return`으로 범위를 벗어나도 실행됩니다.
프로세스 강제 종료나 `Environment.FailFast` 같은 상황까지 모든 경우를 보장하는 것은 아닙니다.
정리 코드 자체에서 예외를 던지면 원래 실패를 가릴 수 있으므로 주의해야 합니다.

## 3. 예제 A: 예외 발생 뒤 실행 순서

```csharp
try
{
    Console.WriteLine("try: start");
    Console.WriteLine(10 / ReadZero());
}
catch (DivideByZeroException)
{
    Console.WriteLine("catch: divide by zero");
}
finally
{
    Console.WriteLine("finally: cleanup");
}
```

첫 출력은 정상적으로 실행됩니다.
`ReadZero()`가 0을 반환하므로 정수 나눗셈에서 예외가 발생합니다.
나눗셈 결과를 받기 전에 실패하므로 둘째 `WriteLine`은 숫자를 출력하지 못합니다.
형식이 맞는 `catch`가 실행됩니다.
이후 `finally`가 실행되고 이 블록 다음의 정상 흐름으로 이어집니다.

메서드로 0을 반환한 것은 상수 식 `10 / 0`의 컴파일 오류와 구분하기 위해서입니다.
이 예제는 실행 중 발생하는 예외 처리를 관찰합니다.
부동소수점 나눗셈은 무한대 등 다른 규칙이 있으므로 정수 예제를 그대로 일반화하지 마세요.

## 4. 예제 B: IDisposable과 using

```csharp
using (DemoResource resource = new("sample"))
{
    resource.Use();
}
Console.WriteLine("after using");
```

생성자에서 `open: sample`을 출력합니다.
본문에서 `use: sample`을 출력합니다.
블록을 벗어날 때 `Dispose()`를 호출하여 `dispose: sample`을 출력합니다.
그 뒤에 `after using`이 출력됩니다.
본문에서 예외가 발생해도 일반적인 예외 전파 과정에서 정리를 수행합니다.

`IDisposable`은 사용을 마친 자원을 정리하는 `Dispose` 계약입니다.
파일 핸들, 연결, 스트림 등 사용을 끝내야 하는 자원에 흔히 사용합니다.
가비지 수집기가 관리 메모리를 회수하는 것과 자원 정리 시점을 정하는 것은 다른 문제입니다.
`Dispose`를 호출했다고 객체 메모리가 즉시 모두 사라지는 것은 아닙니다.
객체의 정리 방식은 해당 형식의 계약을 확인해야 합니다.

```csharp
public void Dispose()
{
    if (_disposed)
    {
        return;
    }

    _disposed = true;
    Console.WriteLine($"dispose: {_name}");
}
```

모형 자원은 여러 번 호출해도 중복 출력을 하지 않도록 상태를 기록합니다.
`Use`는 이미 정리된 상태라면 `ObjectDisposedException`을 발생시킵니다.
학습용 모형이므로 실제 운영체제 핸들이나 finalizer는 사용하지 않습니다.
사용자는 자원 소유권을 확인하고 자신이 정리해야 하는 객체에 `using`을 사용해야 합니다.

C# 8의 `using` 선언은 `using DemoResource resource = new(...);`처럼 씁니다.
별도 중괄호를 만들지 않고 **둘러싼 범위의 끝**에서 정리합니다.
여러 자원을 같은 범위에 선언하면 일반적으로 선언의 역순으로 정리됩니다.
`using System; ` 같은 네임스페이스 지시문과는 역할이 다릅니다.

## 5. 예제 C: 원래 예외를 보존하여 다시 던지기

```csharp
static void ValidateAndRethrow()
{
    try
    {
        ArgumentOutOfRangeException.ThrowIfNegative(-1, "quantity");
    }
    catch (ArgumentOutOfRangeException)
    {
        Console.WriteLine("inner: rethrow");
        throw;
    }
}
```

음수 인수 검증에서 예외가 발생합니다.
안쪽 `catch`는 고정된 안내를 출력하고 `throw;`로 예외를 다시 보냅니다.
바깥 `catch`가 같은 종류의 예외를 잡아 `outer: invalid quantity`를 출력합니다.
`throw;`는 기존 예외의 원래 호출 경로 정보를 유지하는 재전달 방법입니다.
반면 `catch (Exception ex) { throw ex; }`는 재던진 위치에서 스택 정보가 바뀔 수 있습니다.
단순 재전달에서는 `throw;`를 사용하세요.

다른 의미의 예외로 감싸야 한다면 내부 예외를 보존하는 생성자를 검토합니다.
처리 중인 예외를 재전달하는 `throw;`는 `catch` 안에서 사용합니다.
실패 자체를 처리하지 않을 것이라면 불필요한 `catch` 없이 상위로 전파해도 됩니다.

## 6. 전체 기본 출력

```text
try: start
catch: divide by zero
finally: cleanup
open: sample
use: sample
dispose: sample
after using
inner: rethrow
outer: invalid quantity
```

운영체제 언어에 따라 바뀔 수 있는 `Exception.Message`는 기본 출력에 넣지 않았습니다.
이 출력은 예외 형식과 처리 순서 학습에 집중합니다.

## 7. 자주 하는 실수

| 실수 | 이유와 해결 |
|---|---|
| 빈 `catch`로 모든 예외를 무시함 | 실패가 숨겨지므로 처리 목적과 반환 의미를 정합니다. |
| 예외 메시지 문자열로 종류를 판단함 | 지역화될 수 있으므로 예외 형식과 정해진 속성을 사용합니다. |
| `throw ex;`로 원래 예외를 재전달함 | 단순 재전달에는 `throw;`를 사용합니다. |
| GC가 파일을 곧바로 닫는다고 기대함 | 소유한 disposable 자원을 `using`으로 정리합니다. |
| `using` 선언 바로 다음 줄에 정리된다고 생각함 | 선언을 포함하는 범위의 끝에서 정리됩니다. |

## 8. 연습문제

### 문제 1 · 기초: 예상 가능한 실패

문자열 `oops`를 정수로 변환하되 예외 대신 성공 여부를 출력하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번은 `int.TryParse`를 사용합니다.
변환할 수 없으면 false이며 결과 변수는 0입니다.
예상 출력은 `1: False / 0`입니다.

</details>

### 문제 2 · 응용: 실패 전후의 정리 순서

`using` 선언으로 자원을 만든 후 직접 예외를 발생시키세요.
바깥 `catch`보다 `Dispose`가 먼저 실행되는지 확인하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 2번은 `try` 범위 안에서 `using` 선언을 합니다.
예외로 범위를 벗어나면서 자원을 정리한 다음 바깥에서 처리합니다.
순서는 `open: exercise`, `use: exercise`, `dispose: exercise`, `2: recovered`입니다.

</details>

### 문제 3 · 도전: return과 정리

자원을 선언한 메서드에서 42를 반환하세요.
호출자가 값을 출력하는 시점과 정리 시점 중 어느 쪽이 빠른지 확인하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 `ReadAnswer`를 실행합니다.
반환할 값이 결정된 후, 메서드를 완전히 빠져나가기 전에 정리를 수행합니다.
호출자가 반환값을 받는 시점에는 자원이 이미 정리되어 있습니다.
순서는 `open: answer`, `use: answer`, `dispose: answer`, `3: 42`입니다.

</details>

## 9. 복습 질문

1. `catch`가 없어도 `finally`를 사용할 수 있나요? → 네. 정리 후 예외를 상위로 전파할 수 있습니다.
2. 원래 예외를 그대로 다시 전달하는 문법은? → `catch` 안의 `throw;`입니다.
3. `using` 선언은 언제 정리하나요? → 선언을 포함하는 범위를 벗어날 때 정리합니다.

## 공식 문서

- [Microsoft Learn: 예외와 예외 처리](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/)
- [Microsoft Learn: using 문과 선언](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using)
