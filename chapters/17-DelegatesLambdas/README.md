# 17장 · 델리게이트와 람다

[← 16장 Nullable](../16-Nullable/README.md) · [전체 목차](../../README.md) · [18장 이벤트 →](../18-Events/README.md)

## 먼저 알아둘 내용

메서드의 매개변수와 반환형, 제네릭, 리스트, 지역 변수의 범위를 알고 있어야 합니다.
이 장에서는 “숫자나 문자열을 넘긴다”에서 한 걸음 더 나아가 “동작을 넘긴다”는 생각을 익힙니다.
실행 파일은 [Program.cs](Program.cs), 연습문제 정답은 [ExerciseSolutions.cs](ExerciseSolutions.cs)에 있습니다.

## 학습 목표

- 메서드를 담는 형식인 델리게이트의 역할을 설명합니다.
- Action, Func, Predicate의 입력과 반환형을 읽습니다.
- 식 람다, 문 람다, 메서드 그룹을 구분합니다.
- 클로저가 변수를 캡처하는 방식을 예측합니다.
- 반복문에서 공유되는 변수와 별도 지역 변수의 차이를 확인합니다.

## 1. 동작에도 형식이 있다

int 변수에는 정수를 넣습니다. 델리게이트 변수에는 특정한 매개변수와 반환형을 가진 메서드에 대한 참조를 넣습니다.
따라서 델리게이트는 아무 함수나 담는 주머니가 아닙니다. 컴파일러가 호출 계약을 확인하는 형식입니다.
예를 들어 int 하나를 받아 int 하나를 반환하는 동작과 문자열을 받아 출력만 하는 동작은 서로 다른 계약입니다.

델리게이트를 직접 선언할 수도 있습니다.

~~~csharp
delegate int Transform(int value);
~~~

대부분의 일상적인 코드에서는 표준 제네릭 델리게이트를 사용하면 별도 선언이 필요 없습니다.

| 형식 | 의미 | 예 |
| --- | --- | --- |
| Action | 입력 없음, 반환값 없음 | 알림 보내기 |
| Action<string> | 문자열 입력, 반환값 없음 | 로그 출력 |
| Func<int, int> | 정수 입력, 정수 반환 | 제곱 계산 |
| Func<int, int, bool> | 정수 2개 입력, bool 반환 | 두 값 비교 |
| Predicate<int> | 정수 입력, bool 반환 | 조건 판정 |

Func의 마지막 형식 인수는 항상 반환형입니다.
Action에는 반환형 형식 인수를 쓰지 않습니다. 반환값이 void인 계약이 이미 정해져 있기 때문입니다.
Predicate<T>와 Func<T, bool>은 모양이 비슷하지만 서로 다른 델리게이트 형식입니다.
사용하는 API가 어느 형식을 요구하는지 확인해야 합니다.

## 2. 람다와 메서드 그룹

람다는 이름 없는 함수를 간단하게 표현하는 문법입니다.
화살표 왼쪽에는 매개변수, 오른쪽에는 식 또는 실행할 문장을 씁니다.

~~~csharp
Func<int, int> square = static value => value * value;
Predicate<int> isEven = static value => value % 2 == 0;
Action<string> log = Console.WriteLine;
~~~

square는 int를 받아 int를 반환합니다.
isEven의 비교식 결과는 bool이므로 Predicate<int>의 계약과 맞습니다.
Console.WriteLine처럼 메서드 이름을 전달하는 표현을 메서드 그룹이라고 합니다.
컴파일러는 대상 델리게이트 형식을 보고 적절한 오버로드를 선택합니다.

식 하나면 식 람다가 간결합니다. 여러 문장이면 중괄호를 사용합니다.

~~~csharp
Func<int, int> adjusted = value =>
{
    int doubled = value * 2;
    return doubled + 1;
};
~~~

반환값이 있는 문 람다는 모든 정상 종료 경로에서 알맞은 값을 반환해야 합니다.
본문이 짧다는 이유만으로 복잡한 업무 규칙을 긴 람다에 압축할 필요는 없습니다.
이름 있는 메서드로 분리하면 의도를 더 잘 설명할 수 있습니다.

## 3. 실행 예제 A: 표준 델리게이트

Program.cs의 앞부분은 square(3)과 isEven(8)을 호출합니다.
변수에 동작을 저장한 뒤 일반 메서드처럼 괄호를 붙여 호출한다는 점이 핵심입니다.
static 람다는 바깥 지역 변수나 인스턴스 상태를 캡처하지 못하도록 컴파일러가 제한합니다.
캡처가 필요 없는 계산이라면 의도하지 않은 상태 의존성을 줄일 수 있습니다.

## 4. 실행 예제 B: 캡처는 값 복사가 아니다

~~~csharp
int threshold = 10;
Func<int, bool> isLarge = value => value > threshold;
Console.WriteLine(isLarge(12));
threshold = 20;
Console.WriteLine(isLarge(12));
~~~

첫 호출은 True, 두 번째 호출은 False입니다.
람다가 만들어진 순간 threshold 값 10을 고정해서 보관한 것이 아닙니다.
람다 본문이 바깥 변수 threshold에 계속 접근하기 때문입니다.
람다와 그 람다가 접근하는 외부 변수들을 함께 이해하는 개념이 클로저입니다.

이 특성은 설정값을 변경하는 경우에는 유용하지만 지연 실행에서는 예상 밖의 결과를 만들 수 있습니다.
캡처된 변수가 람다보다 짧게 보이는 메서드 안에 선언되어 있어도, 필요하면 그 상태가 계속 살아 있습니다.
객체를 캡처하면 그 객체의 수명도 길어질 수 있습니다.

## 5. 실행 예제 C: 반복문과 변수의 수명

Program.cs의 for문은 shared와 copied에 각각 함수를 저장합니다.
shared의 람다들은 같은 for 변수 i를 읽습니다.
모든 호출이 반복문 종료 후에 일어나므로 그때의 i 값인 3을 세 번 얻습니다.
copied는 반복할 때마다 새 current 변수를 만들고 각각을 캡처합니다.
따라서 0, 1, 2가 유지됩니다.

~~~csharp
for (int i = 0; i < 3; i++)
{
    int current = i;
    copied.Add(() => current);
}
~~~

이 설명을 모든 반복문에 그대로 적용해서는 안 됩니다.
현대 C#의 foreach 반복 변수는 각 반복에서 별도로 취급됩니다.
변수의 선언 위치와 실제 호출 시점을 함께 보아야 합니다.

## 6. 실행 예제 D: 호출 목록

델리게이트는 +=로 호출 목록을 연결할 수 있습니다.
예제의 notify는 first를 출력하고 이어서 second를 출력합니다.
호출은 등록된 순서대로 동기적으로 진행됩니다.
앞의 핸들러에서 예외가 나면 별도 예외 처리 없이는 뒤의 핸들러까지 진행하지 못합니다.
반환값이 있는 멀티캐스트 델리게이트는 마지막 호출의 반환값이 남으므로 결과 집계 용도로 무심코 사용하지 마세요.

## 실행과 정확한 기본 출력

저장소 루트에서 실행합니다.

~~~powershell
dotnet run --project chapters/17-DelegatesLambdas/Chapter17.csproj
dotnet run --project chapters/17-DelegatesLambdas/Chapter17.csproj -- --answers
~~~

인자가 없는 기본 실행 결과입니다. 불리언 출력의 대소문자에도 주목하세요.

~~~text
square(3)=9
even(8)=True
before=True
after=False
shared=3,3,3
copied=0,1,2
first
second
~~~

## 자주 하는 실수와 이유

| 실수 | 왜 문제가 되는가 | 해결 방향 |
| --- | --- | --- |
| Func의 첫 형식을 반환형으로 읽기 | 반환형은 마지막 형식 인수입니다. | 입력들을 먼저 읽고 마지막을 결과로 읽습니다. |
| 람다 생성 시 값이 복사된다고 생각하기 | 외부 변수를 캡처합니다. | 값 고정이 필요하면 별도 지역 변수에 저장합니다. |
| 캡처하면서 static 붙이기 | static 람다는 외부 상태 접근을 금지합니다. | 인수를 추가하거나 static을 의도적으로 제거합니다. |
| 람다 안에서 많은 상태 변경하기 | 호출 횟수와 시점에 따라 결과가 달라집니다. | 가능하면 입력만으로 결과를 계산합니다. |

## 연습문제

### 1단계 · 큰 값 선택

정수 두 개 중 큰 값을 반환하는 Func를 선언하고 7과 4를 전달하세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
Func<int, int, int> max = static (left, right) => Math.Max(left, right);
Console.WriteLine(max(7, 4));
~~~

형식 인수 앞의 둘은 입력, 마지막 int는 반환형입니다.
실행 가능한 정답은 ExerciseSolutions.Run의 첫 부분이며 max=7을 출력합니다.

</details>

### 2단계 · 조건을 전달하는 필터

[3, 8, 12, 5]에서 Predicate<int>로 전달받은 조건을 만족하는 값만 새 리스트에 넣는 Filter를 작성하세요.
호출할 때는 “8 이상” 조건을 전달하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 Filter가 완전한 구현입니다.
반복문은 항목을 방문하는 방법만 담당하고 accept(item)는 선택 정책을 담당합니다.
8,12를 얻으며 조건을 바꾸어도 필터 자체는 변경하지 않습니다.
LINQ를 아직 몰라도 델리게이트로 알고리즘과 정책을 분리할 수 있습니다.

</details>

### 3단계 · 나중에 실행할 작업 만들기

for문으로 Action 3개를 저장하고 반복 종료 후 호출해서 item=1, item=2, item=3을 출력하세요.

<details>
<summary>정답과 해설</summary>

각 반복에서 int captured = i를 선언한 후 해당 변수를 읽는 람다를 추가합니다.
ExerciseSolutions.Run의 마지막 부분을 직접 실행해 확인하세요.
반복 중 즉시 출력하는 것으로 바꾸면 캡처 문제를 검증하지 못합니다.
작업을 저장하는 시점과 실행하는 시점을 반드시 나누어야 합니다.

</details>

## 복습 Q&A

**Q. Action<int>의 결과를 int 변수에 저장할 수 있나요?**  
A. 없습니다. Action은 반환값이 없는 동작입니다.

**Q. 람다를 사용하면 항상 새 스레드가 생기나요?**  
A. 아닙니다. 람다는 동작의 표현 문법이며 실행 스레드는 호출 방식이 결정합니다.

**Q. static 람다가 바깥 상수에도 접근할 수 없나요?**  
A. 상수와 정적 멤버는 사용할 수 있습니다. 캡처가 필요한 지역 변수나 인스턴스 상태가 제한됩니다.

## 공식 문서

- [델리게이트 개요](https://learn.microsoft.com/dotnet/csharp/programming-guide/delegates/)
- [람다 식과 캡처](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions)
- [Func<T,TResult>](https://learn.microsoft.com/dotnet/api/system.func-2)
- [Predicate<T>](https://learn.microsoft.com/dotnet/api/system.predicate-1)
