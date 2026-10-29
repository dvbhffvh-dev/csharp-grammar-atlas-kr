# 23장 · 반복기, 확장 메서드, 튜플과 범위

[← 22장 async와 await](../22-AsyncAwait/README.md) · [전체 목차](../../README.md) · [24장 현대 C# →](../24-ModernCSharp/README.md)

## 먼저 알아둘 내용

IEnumerable<T>, foreach, 메서드, 정적 클래스, 프로퍼티를 복습하세요.
이 장은 자주 만나는 여러 문법을 “언제 평가되는가, 무엇을 반환하는가”라는 관점으로 연결합니다.
완전한 프로그램은 [Program.cs](Program.cs)에 있으며 연습 정답은 [ExerciseSolutions.cs](ExerciseSolutions.cs)에 있습니다.

## 학습 목표

- yield return의 지연 실행과 중단·재개를 설명합니다.
- foreach 종료와 열거자 해제의 관계를 확인합니다.
- 고전적인 this 매개변수 방식으로 확장 메서드를 작성합니다.
- 튜플 반환값을 이름으로 읽거나 분해합니다.
- 인덱스, 범위, 인덱서를 구분합니다.

## 1. 반복기는 값을 필요할 때 만든다

일반 메서드가 List<int>를 만들어 반환하면 반환 전에 목록을 채우는 작업이 일어납니다.
반복기 메서드는 IEnumerable<int>와 yield return으로 원소를 차례로 제공할 수 있습니다.
호출하는 순간 모든 원소를 만든다는 뜻이 아닙니다.
소비자가 다음 원소를 요청할 때 메서드 본문이 필요한 지점까지 실행됩니다.

~~~csharp
static IEnumerable<int> Generate()
{
    yield return 10;
    yield return 20;
}
~~~

첫 원소를 요청하면 10을 제공하고 상태를 보관합니다.
다음 원소를 요청하면 앞의 yield return 다음부터 이어져 20을 제공합니다.
yield break는 반복기에서 더 이상 원소를 제공하지 않고 종료한다는 뜻입니다.
일반 return 값과 yield return을 같은 반복기 본문에서 섞어 쓰지는 않습니다.

반복기를 두 번 열거하면 생성 로직도 두 번 실행될 수 있습니다.
항목이 파일 읽기나 계산 결과라면 다시 실행하는 비용과 데이터 변화가 생길 수 있습니다.
고정된 결과를 재사용하려면 적절한 시점에 ToArray 같은 구체화를 고려합니다.

## 2. 실행 예제 A: 생성과 열거 시점 분리

~~~csharp
IEnumerable<int> sequence = Generate();
Console.WriteLine("created");
foreach (int value in sequence)
{
    Console.WriteLine($"value={value}");
    break;
}
~~~

실행 결과에서 created가 iterator-start보다 먼저 나타납니다.
Generate 호출은 열거할 수 있는 객체를 반환하지만 본문 실행은 첫 열거 시점에 시작되기 때문입니다.
첫 번째 원소 10을 출력한 뒤 break하므로 두 번째 원소 20은 생성 흐름에 도달하지 않습니다.

Program.cs의 Generate에는 try/finally가 있습니다.
foreach를 break로 끝내도 열거자를 Dispose하므로 finally의 iterator-disposed가 출력됩니다.
반복기가 내부에서 사용하는 파일 같은 리소스의 수명과 연결되는 중요한 특성입니다.

직접 GetEnumerator를 호출했다면 직접 수명 관리도 책임져야 합니다.
열거자를 만들어 놓고 Dispose 없이 버리는 방식은 정리 시점을 흐리게 만듭니다.
foreach 또는 using으로 일반적인 정리 규칙을 활용하세요.
yield return은 catch가 있는 try나 catch/finally 본문 등에 사용할 수 없는 제약이 있으므로 예외 구조도 확인해야 합니다.
이 예제처럼 catch 없는 try/finally 안에서 yield return을 쓰는 것은 가능합니다.

## 3. 실행 예제 B: 기존 형식에 읽기 좋은 호출 제공

~~~csharp
public static class NumberExtensions
{
    public static int ClampTo(this int value, int minimum, int maximum)
        => Math.Clamp(value, minimum, maximum);
}
~~~

고전적인 확장 메서드는 정적 클래스 안의 정적 메서드로 선언합니다.
첫 매개변수에 this를 붙이면 해당 형식의 인스턴스 메서드처럼 호출할 수 있습니다.

~~~csharp
int bounded = 12.ClampTo(0, 10);
~~~

이는 NumberExtensions.ClampTo(12, 0, 10)처럼 정적 메서드를 호출하는 편의 문법입니다.
int 형식 자체의 소스나 실제 멤버가 바뀌는 것은 아닙니다.
확장 메서드는 대상 형식의 private 멤버에 특별히 접근할 권한도 없습니다.

사용하려는 확장 메서드의 네임스페이스가 범위에 있어야 합니다.
일치하는 인스턴스 메서드가 있으면 일반적으로 인스턴스 메서드가 우선됩니다.
메서드 이름을 기존 API와 비슷하게 만들수록 동작 의미도 일관되게 유지하세요.

참조 형식에 대한 확장 메서드는 null 수신자에 대해서도 정적 호출로 진입할 수 있습니다.
메서드 내부에서 null 처리 정책을 명시해야 하며 “메서드처럼 보이니 이미 null 검사되었다”고 가정하면 안 됩니다.
이 예제는 값 형식 int에 적용하므로 그런 null 상황은 없습니다.

## 4. 실행 예제 C: 여러 결과를 튜플로 반환하기

~~~csharp
static (int Min, int Max) MinMax(int[] values)
    => (values.Min(), values.Max());

(int minimum, int maximum) = MinMax([7, 2, 9]);
~~~

튜플은 몇 개의 관련 값을 간단히 묶어 전달하는 방법입니다.
반환 형식에 Min과 Max라는 요소 이름을 붙이면 의미를 읽기 쉽습니다.
분해 선언은 각 값을 별도 지역 변수로 받습니다.
var result로 받은 뒤 result.Min처럼 접근할 수도 있습니다.

필요 없는 요소는 var (_, total) = ...처럼 버림 기호 _로 생략할 수 있습니다.
이름은 읽기 편의를 제공하며 요소 순서와 형식의 중요성을 없애지는 않습니다.
외부 API에서 장기간 의미가 유지되어야 하는 복잡한 데이터라면 전용 클래스나 레코드가 더 적절할 수 있습니다.

실제 Program.cs는 빈 배열을 먼저 검사합니다.
최솟값과 최댓값은 원소가 하나 이상 있어야 의미가 있으므로 입력 계약을 명확하게 한 것입니다.
짧은 문법을 쓰더라도 경계 조건 처리는 생략하지 않습니다.

## 5. 실행 예제 D: Index, Range와 인덱서

~~~csharp
string[] words = ["zero", "one", "two", "three"];
string last = words[^1];
string[] middle = words[1..3];
~~~

^1은 뒤에서 첫 번째 위치이므로 마지막 원소입니다.
^0은 마지막 원소가 아니라 컬렉션 끝의 경계 위치입니다.
따라서 words[^0]처럼 원소를 읽으려 하면 범위를 벗어납니다.
0..^0처럼 범위의 끝 경계로 사용하는 것은 가능합니다.

범위의 시작은 포함하고 끝은 제외합니다.
[1..3]은 인덱스 1과 2를 포함하므로 one,two입니다.
배열에 범위를 적용한 결과는 새 배열이며 저장 공간과 복사 비용이 듭니다.
참조 형식 원소의 객체 내부까지 복제하는 것은 아닙니다.
Span<T>의 슬라이스는 같은 저장 공간을 바라보는 방식이므로 배열 범위와 비용을 혼동하지 마세요.

인덱서는 객체에 [] 접근 문법을 제공하는 멤버입니다.

~~~csharp
public string this[int index]
{
    get => _books[index];
    set => _books[index] = value;
}
~~~

Shelf는 자체 내부 배열을 보관하고 []를 통해 책을 읽고 바꿉니다.
내부 배열 접근이 범위를 확인하므로 잘못된 인덱스는 예외가 됩니다.
인덱서는 반드시 정수 하나만 받는 것은 아니며 매개변수와 반환형을 설계할 수 있습니다.
이 Shelf에 Range를 지원하는 인덱서를 선언하지 않았으므로 배열처럼 모든 범위 문법을 자동 지원한다고 가정하지 마세요.

## 실행과 정확한 기본 출력

~~~powershell
dotnet run --project chapters/23-AdvancedSyntax/Chapter23.csproj
dotnet run --project chapters/23-AdvancedSyntax/Chapter23.csproj -- --answers
~~~

~~~text
created
iterator-start
value=10
iterator-disposed
extension=10
tuple=2,9
last=three
slice=one,two
indexer=LINQ
updated=Async
~~~

## 자주 하는 실수와 이유

| 실수 | 실제 의미 | 해결 방향 |
| --- | --- | --- |
| 반복기 호출 시 본문이 전부 실행된다고 생각하기 | 열거할 때 진행됩니다. | 호출과 소비 시점을 구분합니다. |
| break하면 finally를 건너뛴다고 생각하기 | foreach가 열거자를 해제합니다. | 실행 출력으로 수명을 확인합니다. |
| 확장 메서드로 private 멤버에 접근하기 | 일반 정적 메서드와 같은 접근 제한입니다. | 공개 API를 사용합니다. |
| 범위의 끝도 포함된다고 생각하기 | 끝은 제외합니다. | 결과 길이를 끝-시작으로 확인합니다. |
| ^0을 마지막 항목으로 사용하기 | 끝 경계입니다. | 마지막 원소는 ^1입니다. |

## 연습문제

### 1단계 · 짝수 반복기

0부터 짝수를 차례로 제공하고 Take(4)로 처음 네 개만 출력하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 EvenNumbers는 yield return을 사용합니다.
Take(4)가 소비를 멈추므로 0,2,4,6만 출력됩니다.
int 덧셈이 범위를 넘어가지 않도록 반복 상한도 고려했습니다.

</details>

### 2단계 · 개수와 합계

배열을 받아 (Count, Sum)을 반환하고 튜플 분해로 출력하세요.
두 번째 호출에서는 개수를 _로 버려 보세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
private static (int Count, int Sum) Summarize(int[] values)
    => (values.Length, values.Sum());
~~~

정답은 summary=3,12와 discard=4를 출력합니다.
분해 변수의 이름이 반환 튜플 요소의 이름과 같을 필요는 없습니다.

</details>

### 3단계 · 배열 범위의 독립성

[10, 20, 30, 40]에서 [1..^1]로 중간 두 항목을 얻고 첫 값을 99로 바꾸세요.
원본이 어떻게 되는지 확인하세요.

<details>
<summary>정답과 해설</summary>

배열 범위 결과는 새 배열이므로 middle=99,30과 original=10,20,30,40이 출력됩니다.
원소가 정수인 이 예제는 저장 공간 독립성이 직접 보입니다.
객체 참조 배열에서는 배열 복사와 객체 복사를 구분해야 합니다.

</details>

## 복습 Q&A

**Q. yield return은 메서드를 완전히 끝내나요?**  
A. 원소 하나를 제공하고 다음 요청까지 중단합니다. 다음 요청에서 이어집니다.

**Q. 확장 메서드는 원래 형식의 상속 계층을 바꾸나요?**  
A. 아닙니다. 정적 메서드를 인스턴스 호출처럼 사용할 수 있게 합니다.

**Q. [1..3]은 몇 개의 원소인가요?**  
A. 두 개입니다. 시작은 포함하고 끝은 제외합니다.

## 공식 문서

- [반복기](https://learn.microsoft.com/dotnet/csharp/iterators)
- [yield 문](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/yield)
- [확장 멤버와 기존 확장 메서드](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
- [튜플](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples)
- [인덱스와 범위](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators)
- [인덱서](https://learn.microsoft.com/dotnet/csharp/programming-guide/indexers/)
