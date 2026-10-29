# 18장 · 이벤트와 구독 수명

[← 17장 델리게이트와 람다](../17-DelegatesLambdas/README.md) · [전체 목차](../../README.md) · [19장 LINQ →](../19-Linq/README.md)

## 먼저 알아둘 내용

17장의 델리게이트, 람다, 캡처를 이해하고 있어야 합니다.
클래스, 접근 제한자, nullable 참조 형식, IDisposable의 기초도 사용합니다.
[Program.cs](Program.cs)는 온도 센서와 두 구독자를 연결하는 완전한 실행 예제입니다.
[ExerciseSolutions.cs](ExerciseSolutions.cs)에는 구독 수명까지 관리하는 정답이 있습니다.

## 학습 목표

- 발행자와 구독자의 책임을 나눕니다.
- event가 일반 델리게이트 멤버와 다른 이유를 설명합니다.
- EventHandler<TEventArgs>로 변경 전후 데이터를 전달합니다.
- 같은 핸들러를 사용해 구독과 구독 해제를 연결합니다.
- 이벤트가 동기적으로 호출된다는 기본 동작을 이해합니다.

## 1. 누가 누구를 알아야 할까?

온도 센서는 현재 온도를 관리합니다.
화면은 온도 변화를 보여 주고 경보 장치는 기준 온도 이상이면 알림을 표시합니다.
센서가 화면 클래스나 경보 클래스를 직접 생성하면 센서가 여러 표시 방식에 의존하게 됩니다.
이벤트를 사용하면 센서는 “온도가 바뀌었다”는 사실을 알리고, 구독자는 자신의 동작을 등록합니다.

발행자는 이벤트를 선언하고 발생시키는 객체입니다.
구독자는 +=로 핸들러를 등록하고 필요 없어지면 -=로 해제하는 객체 또는 코드입니다.
구독자가 없다는 것도 정상 상태이므로 센서 자체의 상태 변경은 계속 동작해야 합니다.

## 2. event가 보호하는 경계

~~~csharp
public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;
~~~

EventHandler<T>는 sender와 이벤트 데이터를 받는 델리게이트입니다.
sender에는 일반적으로 이벤트를 발생시킨 객체를 넣습니다.
이 예제의 데이터 형식은 이전 온도와 새 온도를 읽기 전용 프로퍼티로 제공합니다.
EventArgs 상속은 .NET에서 널리 쓰는 이벤트 설계 관례입니다.

event가 붙으면 외부 코드는 이 멤버에 구독하거나 구독을 해제할 수 있습니다.
외부 코드가 전체 호출 목록을 덮어쓰거나 직접 이벤트를 발생시키는 것은 허용되지 않습니다.
이벤트를 선언한 형식이 상태와 알림 시점을 함께 관리하도록 하는 제한입니다.
일반 public 델리게이트 필드로 작성하면 이 경계가 사라집니다.

이벤트에 붙은 ?는 아직 구독자가 없을 수 있다는 뜻입니다.
발행할 때 ?.Invoke를 사용하면 null인 호출 목록을 안전하게 건너뜁니다.

## 3. 실행 예제 A: 상태를 변경하고 알리기

~~~csharp
if (Temperature == value)
{
    return;
}

int oldValue = Temperature;
Temperature = value;
TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(oldValue, value));
~~~

같은 온도를 다시 설정하면 실제 변경이 없으므로 알림을 생략합니다.
이것은 이벤트 언어 기능이 자동으로 제공하는 동작이 아니라 센서가 정한 정책입니다.
상태 변경 후 이벤트를 발생시키므로 핸들러가 센서의 Temperature를 읽어도 새 값을 관찰합니다.
변경 전후 값은 이벤트 데이터 안에 담아 구독자가 과거 상태를 따로 저장할 필요를 줄입니다.

프로퍼티의 private set은 센서 외부에서 Temperature를 직접 바꾸지 못하게 합니다.
모든 변경을 SetTemperature에 모아야 검증과 알림이 빠지지 않습니다.

## 4. 실행 예제 B: 두 구독자의 서로 다른 역할

~~~csharp
EventHandler<TemperatureChangedEventArgs> display =
    (_, change) => Console.WriteLine($"temperature={change.OldValue}->{change.NewValue}");

sensor.TemperatureChanged += display;
sensor.TemperatureChanged += alert;
~~~

display는 모든 변경을 출력합니다.
alert는 25도 이상으로 변경될 때만 출력합니다.
사용하지 않는 sender는 _라는 이름으로 명시합니다.
첫 변화 0 → 22에서는 display만 출력하고, 22 → 25에서는 display와 alert가 차례로 출력합니다.

이 예제의 이벤트 발생은 동기 호출입니다.
SetTemperature는 각 핸들러가 끝날 때까지 반환하지 않습니다.
이벤트를 사용했다는 이유로 별도 스레드에서 자동 실행되지는 않습니다.
긴 작업을 핸들러에 넣으면 발행자도 오래 기다리게 됩니다.

## 5. 실행 예제 C: 같은 핸들러로 해제하기

~~~csharp
sensor.TemperatureChanged -= alert;
sensor.SetTemperature(30);
sensor.TemperatureChanged -= display;
sensor.SetTemperature(35);
~~~

alert를 해제한 뒤에는 30도에서도 경보 출력이 없습니다.
display까지 해제한 뒤에는 35도로 바뀌어도 이벤트 출력은 없습니다.
마지막 final=35는 구독자가 없어도 센서 상태가 정상적으로 변경되었음을 보여 줍니다.

람다를 사용해서 구독했다면 나중에 해제할 때도 같은 델리게이트를 사용해야 합니다.
본문이 같아 보이는 새 람다를 -=에 쓰는 방식은 원래 핸들러 해제를 보장하지 못합니다.
따라서 예제는 람다를 변수에 저장합니다.
이름 있는 인스턴스 메서드를 핸들러로 쓰는 것도 좋은 방법입니다.

## 6. 구독은 객체 수명에 영향을 준다

발행자의 호출 목록은 구독자의 인스턴스 메서드를 참조할 수 있습니다.
발행자가 오래 살아 있으면 더 이상 화면에 보이지 않는 구독자도 계속 참조될 수 있습니다.
이벤트가 필요 없어진 시점에 해제하지 않으면 예상보다 객체 수명이 길어질 수 있습니다.

ExerciseSolutions.cs의 ThresholdListener는 생성자에서 구독하고 Dispose에서 해제합니다.
using 블록의 종료와 함께 구독도 끝나도록 구성한 것입니다.
Dispose를 두 번 호출해도 같은 해제 작업이 반복되지 않도록 _disposed로 관리합니다.
모든 이벤트 구독에 반드시 IDisposable이 필요한 것은 아니지만 수명 경계가 분명할수록 관리가 쉽습니다.

## 실행과 정확한 기본 출력

저장소 루트에서 실행합니다.

~~~powershell
dotnet run --project chapters/18-Events/Chapter18.csproj
dotnet run --project chapters/18-Events/Chapter18.csproj -- --answers
~~~

~~~text
temperature=0->22
temperature=22->25
alert=25
temperature=25->30
final=35
~~~

## 자주 하는 실수와 이유

| 실수 | 원인과 영향 | 개선 방법 |
| --- | --- | --- |
| 외부에서 이벤트를 호출하려고 하기 | 발행 권한이 선언 형식 내부에 있습니다. | 상태 변경 메서드를 호출합니다. |
| 구독할 때마다 새 람다를 만들고 해제는 생략하기 | 호출 중복과 객체 수명 증가가 생길 수 있습니다. | 핸들러를 저장하고 수명 종료에 해제합니다. |
| +=를 같은 핸들러에 여러 번 호출하기 | 호출 목록에 중복 등록될 수 있습니다. | 등록 경로가 한 번인지 확인합니다. |
| 하나의 핸들러 예외가 다른 핸들러와 무관하다고 믿기 | 기본 동기 호출은 예외로 중단됩니다. | 예외 정책을 설계하고 핸들러를 작게 유지합니다. |
| 이벤트가 스레드 안전성을 보장한다고 믿기 | 상태 읽기·쓰기와 구독 수명은 별도 문제입니다. | 여러 스레드가 필요할 때 명시적으로 동기화합니다. |

이 예제는 단일 실행 흐름으로 동작합니다.
?.Invoke는 구독자가 없는 경우를 처리하는 표현이며 센서 전체의 동시성 문제를 해결하지 않습니다.

## 연습문제

### 1단계 · 구독자 없는 센서

새 센서를 만들고 구독 없이 온도를 10으로 변경하세요.
예외 없이 Temperature가 10이 되는지 확인하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.Run의 quiet 센서 부분을 실행하세요.
TemperatureChanged?.Invoke가 구독자 없는 경우를 건너뛰기 때문에 without-subscriber=10이 출력됩니다.
이벤트와 센서의 핵심 상태 변경이 독립적으로 동작합니다.

</details>

### 2단계 · 호출 횟수 검증

핸들러 하나를 구독하고 10, 10, 20 순서로 설정하세요.
해제한 뒤 30으로 설정했을 때 총 호출 횟수는 얼마인가요?

<details>
<summary>정답과 해설</summary>

~~~csharp
int calls = 0;
EventHandler<TemperatureChangedEventArgs> count = (_, _) => calls++;
~~~

답은 2입니다.
두 번째 10은 상태 변화가 없고 30을 설정할 때는 이미 해제되었습니다.
전체 실행 코드는 ExerciseSolutions.cs에 있습니다.

</details>

### 3단계 · 수명을 관리하는 구독자

IDisposable을 구현한 ThresholdListener를 만들고 using 안에서만 경보를 받으세요.
기준은 40이며 40, 45, using 종료 후 50을 설정하세요.

<details>
<summary>정답과 해설</summary>

정답 파일의 ThresholdListener가 완전한 구현입니다.
생성자에서 Handle을 구독하고 Dispose에서 같은 Handle을 해제합니다.
threshold=40과 threshold=45만 출력되어야 합니다.
50까지 출력된다면 해제 시점 또는 핸들러 참조를 점검하세요.

</details>

## 복습 Q&A

**Q. 이벤트는 구독자가 꼭 있어야 하나요?**  
A. 아닙니다. 구독이 없는 상태도 정상적으로 처리하도록 작성합니다.

**Q. 이벤트를 발생시키면 핸들러가 동시에 실행되나요?**  
A. 기본 델리게이트 호출은 같은 실행 흐름에서 순서대로 동작합니다.

**Q. 같은 핸들러를 두 번 등록한 뒤 한 번 해제하면 어떻게 되나요?**  
A. 일치하는 마지막 등록 하나를 제거합니다. 중복 구독 자체를 피하는 편이 명확합니다.

## 공식 문서

- [이벤트 개요](https://learn.microsoft.com/dotnet/csharp/events-overview)
- [이벤트 구독과 해제](https://learn.microsoft.com/dotnet/csharp/programming-guide/events/how-to-subscribe-to-and-unsubscribe-from-events)
- [표준 .NET 이벤트 패턴](https://learn.microsoft.com/dotnet/csharp/event-pattern)
- [EventHandler<TEventArgs>](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)
