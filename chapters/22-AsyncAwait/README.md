# 22장 · Task, async, await와 취소

[← 21장 파일과 JSON](../21-FilesJson/README.md) · [전체 목차](../../README.md) · [23장 확장 문법 →](../23-AdvancedSyntax/README.md)

## 먼저 알아둘 내용

메서드 반환형, 예외 처리, 람다, IDisposable을 알고 있어야 합니다.
이 장의 “기다림”은 작업이 끝날 때까지 현재 스레드를 무조건 붙잡는 것과 다릅니다.
실제 네트워크 없이 Task.Delay로 비동기 대기를 재현합니다.
출력은 작업 완료 순서나 컴퓨터 속도에 의존하지 않도록 구성했습니다.

## 학습 목표

- Task와 Task<T>가 나타내는 작업의 의미를 설명합니다.
- await 이후 코드가 언제 이어지는지 이해합니다.
- 독립 작업을 시작한 뒤 Task.WhenAll로 기다립니다.
- CancellationToken을 하위 작업까지 전달합니다.
- I/O 대기와 CPU 계산의 차이를 구분합니다.

## 1. Task는 작업의 완료를 표현한다

Task는 결과값 없이 완료·실패·취소를 표현하는 객체입니다.
Task<T>는 정상 완료 시 T 형식의 결과도 제공합니다.
Task<string>은 문자열 자체가 아니라 나중에 문자열을 얻을 수 있는 작업을 나타냅니다.
await task의 결과를 통해 문자열을 얻습니다.

~~~csharp
static async Task<string> LoadLabelAsync(
    string label, int delay, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    await Task.Delay(delay, cancellationToken);
    return $"loaded-{label}";
}
~~~

async는 메서드 안에서 await를 사용할 수 있도록 하는 언어 기능입니다.
async를 붙였다고 메서드 전체가 자동으로 다른 스레드에서 실행되는 것은 아닙니다.
일반적으로 메서드는 호출 스레드에서 시작하여 아직 끝나지 않은 await를 만날 때 제어를 돌려줍니다.
기다리는 작업이 이미 끝났다면 그 await에서 실제 중단 없이 계속 진행할 수도 있습니다.

컴파일러는 메서드의 진행 위치와 필요한 지역 상태를 보관하는 코드를 만들어 줍니다.
이 때문에 비동기 흐름을 순차적인 문장처럼 읽을 수 있습니다.
내부 변환을 직접 구현할 필요는 없지만 호출과 완료가 별개라는 점은 이해해야 합니다.

## 2. I/O와 CPU를 나누어 생각하기

파일 읽기나 네트워크 요청은 외부 장치의 응답을 기다리는 시간이 큽니다.
비동기 API를 사용하면 대기 동안 호출 스레드를 다른 일에 사용할 수 있습니다.
비동기가 외부 장치의 속도 자체를 반드시 높여 주는 것은 아닙니다.

CPU 계산은 실제로 프로세서 시간을 사용합니다.
UI 프로그램에서 긴 계산을 분리할 때 Task.Run으로 스레드 풀에 맡길 수 있습니다.
서버에서 모든 계산을 무작정 Task.Run으로 감싼다고 처리량이 좋아지는 것은 아닙니다.
스레드와 스케줄링 비용을 고려해야 합니다.

Program.cs의 cpu-sum은 Task.Run의 형태를 보여 주는 작은 예제입니다.
1부터 5까지의 합처럼 짧은 계산에 Task.Run이 성능상 필요하다는 뜻은 아닙니다.
실제 I/O 비동기 API가 있다면 불필요하게 Task.Run으로 감싸지 않고 직접 await하는 경우가 많습니다.

## 3. 실행 예제 A: 독립 작업을 함께 기다리기

~~~csharp
Task<string> first = LoadLabelAsync("A", 20, CancellationToken.None);
Task<string> second = LoadLabelAsync("B", 1, CancellationToken.None);
string[] labels = await Task.WhenAll(first, second);
~~~

두 메서드를 먼저 호출하므로 두 대기가 겹칠 수 있습니다.
첫 작업을 곧바로 await한 뒤 두 번째 작업을 호출하면 흐름이 직렬로 이어집니다.
두 작업이 독립적인 경우라면 먼저 둘 다 시작하고 함께 기다릴 수 있습니다.

Task.WhenAll은 전달한 작업이 모두 완료될 때 완료되는 새 작업을 반환합니다.
Task<T>들의 결과 배열은 완료 순서가 아니라 입력 작업 순서입니다.
따라서 B의 대기가 더 짧더라도 예제 출력은 loaded-A,loaded-B입니다.
작업 내부에서 즉시 출력했다면 출력 순서는 달라질 수 있으므로 결과 수집 후 한 번에 출력합니다.

동시에 많은 작업을 시작할 때는 메모리, 연결 수, 외부 서비스 제한도 고려해야 합니다.
WhenAll이 알아서 적절한 동시 실행 개수를 제한해 주지는 않습니다.
이 예제는 두 개의 짧은 작업만 사용합니다.

## 4. 실행 예제 B: 협력적 취소

~~~csharp
using CancellationTokenSource source = new();
source.Cancel();
try
{
    await LoadLabelAsync("C", 1, source.Token);
}
catch (OperationCanceledException) when (source.IsCancellationRequested)
{
    Console.WriteLine("canceled=True");
}
~~~

CancellationTokenSource는 취소를 요청하는 쪽이고 CancellationToken은 요청 상태를 전달받는 쪽입니다.
Cancel은 작업을 강제로 종료시키거나 스레드를 중단하는 명령이 아닙니다.
작업이 토큰을 검사하거나 토큰을 지원하는 API에 전달해야 취소에 협력할 수 있습니다.

예제는 시작 전에 Cancel을 호출하여 취소 경로를 항상 재현합니다.
실제 UI에서는 사용자가 취소 버튼을 눌렀을 때 source.Cancel을 호출하는 식으로 연결할 수 있습니다.
ThrowIfCancellationRequested와 Task.Delay에 같은 토큰을 전달하여 검사를 이어 갑니다.
상위 메서드만 토큰을 받고 하위 작업에 넘기지 않으면 실제 대기가 계속될 수 있습니다.

OperationCanceledException은 일반적인 비동기 취소를 처리하는 기준 예외입니다.
실제 API에서는 그 하위 형식인 TaskCanceledException을 볼 수도 있습니다.
정리 작업은 using 또는 finally로 처리하여 성공·실패·취소에서 모두 실행되도록 합니다.

## 5. 실행 예제 C: await에서 예외 관찰하기

FailAsync는 Task.Yield 후 InvalidOperationException을 발생시킵니다.
호출 쪽은 await를 try 안에 두고 해당 예외를 처리합니다.
작업을 만들기만 하고 기다리지 않으면 완료와 실패를 적절한 곳에서 관찰하기 어렵습니다.

WhenAll에 여러 실패가 있다면 전체 작업의 Exception에는 여러 원인이 담길 수 있습니다.
await가 던지는 예외 하나만으로 모든 작업의 실패 목록을 얻었다고 생각하지 마세요.
여러 실패를 모두 보고해야 하는 시스템에서는 WhenAll 작업과 개별 작업 상태를 함께 검사하는 정책이 필요합니다.

## 6. async void와 동기 대기 주의

일반 비동기 메서드는 Task 또는 Task<T>를 반환하는 방식이 적절합니다.
async void는 호출자가 await할 수 없고 예외를 일반 Task처럼 추적할 수 없습니다.
반환형이 void여야 하는 이벤트 핸들러는 대표적인 예외 상황입니다.
그 경우에도 핸들러 내부에서 오류를 명시적으로 처리하고 핵심 로직은 Task 반환 메서드로 분리하는 편이 좋습니다.

.Result나 .Wait()는 호출 스레드를 막습니다.
특정 UI 동기화 컨텍스트에서는 비동기 작업이 같은 컨텍스트로 돌아오려다 교착 상태가 생길 수 있습니다.
가능하면 호출 흐름 전체에서 await를 이어 사용하세요.
ConfigureAwait 같은 세부 정책은 라이브러리와 UI 문맥을 이해한 뒤 다루는 주제입니다.

## 실행과 정확한 기본 출력

이 장의 정답 진입점만 ExerciseSolutions.RunAsync()이며 Program.cs에서 await합니다.

~~~powershell
dotnet run --project chapters/22-AsyncAwait/Chapter22.csproj
dotnet run --project chapters/22-AsyncAwait/Chapter22.csproj -- --answers
~~~

~~~text
results=loaded-A,loaded-B
cpu-sum=15
canceled=True
error=sample
~~~

## 자주 하는 실수와 이유

| 실수 | 실제 동작 | 개선 방법 |
| --- | --- | --- |
| async면 새 스레드라고 생각하기 | 언어의 중단·재개 기능입니다. | 실행 API와 작업 종류를 확인합니다. |
| 독립 작업을 하나씩 바로 await하기 | 작업 대기가 순서대로 이어집니다. | 먼저 시작하고 WhenAll을 사용합니다. |
| 토큰을 받기만 하고 무시하기 | 취소 요청이 실제 작업에 전달되지 않습니다. | 하위 API까지 전달합니다. |
| 작업을 버리고 성공했다고 처리하기 | 완료와 예외를 놓칩니다. | await하거나 명확한 작업 수명 관리자를 둡니다. |
| 모든 비동기 메서드를 async void로 만들기 | 호출자가 기다리고 실패를 관찰하기 어렵습니다. | Task 반환을 기본으로 합니다. |

## 연습문제

### 1단계 · 비동기 두 배 계산

Task.Delay 뒤에 입력 정수의 두 배를 반환하는 Task<int> 메서드를 작성하세요.
6을 전달하고 결과 12를 출력하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 DoubleAsync를 실행하세요.
이 계산 자체에는 비동기가 필요하지 않지만 await와 반환형을 연습하기 위해 짧은 대기를 넣었습니다.
호출자는 await DoubleAsync(...)의 int 결과를 사용합니다.

</details>

### 2단계 · 결과 순서 확인

2와 5에 대한 DoubleAsync 작업을 먼저 시작한 뒤 WhenAll로 결과를 받으세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
Task<int>[] work =
[
    DoubleAsync(2, CancellationToken.None),
    DoubleAsync(5, CancellationToken.None)
];
int[] results = await Task.WhenAll(work);
~~~

정답은 batch=4,10입니다.
결과 배열의 위치는 전달한 작업의 위치와 대응합니다.

</details>

### 3단계 · 취소 전달

미리 취소된 토큰을 DoubleAsync에 전달하고 취소를 처리하세요.

<details>
<summary>정답과 해설</summary>

CancellationTokenSource를 만들고 Cancel 후 Token을 전달합니다.
DoubleAsync는 Task.Delay에 토큰을 전달하므로 취소된 작업을 await하게 됩니다.
정답은 answer-canceled를 출력하며 결과값은 출력하지 않습니다.

</details>

## 복습 Q&A

**Q. await는 스레드를 계속 붙잡고 기다리나요?**  
A. 미완료 비동기 작업을 기다릴 때는 제어를 돌려주고 완료 후 나머지를 이어 갑니다.

**Q. Cancel을 호출하면 작업이 즉시 강제 종료되나요?**  
A. 아닙니다. 작업이 취소 요청에 협력해야 합니다.

**Q. WhenAll의 결과 배열은 빨리 끝난 순서인가요?**  
A. 아닙니다. 입력 작업 순서입니다.

## 공식 문서

- [비동기 프로그래밍 개요](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/)
- [비동기 프로그래밍 시나리오](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/async-scenarios)
- [Task.WhenAll](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.whenall)
- [관리 스레드의 취소](https://learn.microsoft.com/dotnet/standard/threading/cancellation-in-managed-threads)
- [비동기 반환 형식](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/async-return-types)
