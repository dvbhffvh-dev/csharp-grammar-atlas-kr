# 20장 · 레코드, with, 패턴 매칭

[← 19장 LINQ](../19-Linq/README.md) · [전체 목차](../../README.md) · [21장 파일과 JSON →](../21-FilesJson/README.md)

## 먼저 알아둘 내용

클래스와 구조체, 참조 형식과 값 형식, 프로퍼티, switch, nullable을 복습하세요.
이 장은 데이터의 “동등함”과 “형태에 따른 분기”를 연결합니다.
실행 예제는 [Program.cs](Program.cs), 연습 정답은 [ExerciseSolutions.cs](ExerciseSolutions.cs)입니다.

## 학습 목표

- record의 값 기반 동등성과 참조 동일성을 구분합니다.
- with가 만드는 얕은 복사의 영향을 설명합니다.
- 관계, 논리, 프로퍼티, 목록 패턴을 읽습니다.
- switch 식에서 순서와 마지막 기본 처리의 중요성을 이해합니다.
- 불변 프로퍼티와 깊은 불변성의 차이를 설명합니다.

## 1. 같은 객체인가, 같은 데이터인가?

일반적인 클래스에서 두 참조의 == 비교는 별도 연산자 정의가 없다면 같은 객체를 가리키는지 검사합니다.
레코드는 데이터 중심 형식에 적합한 동등성 구현을 컴파일러가 만들어 줍니다.
record만 쓰면 record class이며 참조 형식입니다.
record struct는 값 형식이므로 저장과 복사 방식도 함께 고려해야 합니다.

~~~csharp
public sealed record Point(int X, int Y);
Point first = new(2, 3);
Point second = new(2, 3);
~~~

first와 second는 서로 다른 인스턴스지만 데이터 값은 같습니다.
따라서 first == second는 True이고 ReferenceEquals(first, second)는 False입니다.
“값 기반 비교를 한다”와 “값 형식이다”는 서로 다른 개념입니다.

위 위치 기반 record class 선언은 X와 Y에 대응하는 공개 init 프로퍼티도 만들어 줍니다.
일반 class의 주 생성자와 다르게 프로퍼티 생성 규칙이 있다는 점은 24장에서 다시 다룹니다.
값 동등성에는 레코드의 실제 형식도 관련되므로 상속 관계의 서로 다른 레코드를 단순 필드 목록만으로 판단하지 마세요.

## 2. 실행 예제 A: with로 변경본 만들기

~~~csharp
Book original = new("C#", 30, ["beginner"]);
Book revised = original with { Title = "C# Guide" };
~~~

with 식은 원본의 상태를 기반으로 새 인스턴스를 만들고 지정한 멤버의 값을 바꿉니다.
original.Title은 C#이고 revised.Title은 C# Guide입니다.
이 방식은 원본을 덮어쓰지 않고 새로운 버전의 데이터를 만드는 데 유용합니다.

하지만 이 과정은 기본적으로 얕은 복사입니다.
Book에는 문자열 배열 Tags가 있으며 배열은 참조 형식입니다.
따라서 두 Book의 Tags가 같은 배열을 가리킬 수 있습니다.

~~~csharp
revised.Tags[0] = "advanced";
Console.WriteLine(original.Tags[0]);
~~~

결과는 advanced입니다.
init 프로퍼티는 생성 후 그 프로퍼티에 다른 배열을 대입하는 것을 제한할 뿐 배열 내부 수정까지 막지 않습니다.
읽기 전용 프로퍼티와 데이터 전체가 변경 불가능한 상태는 같지 않습니다.

## 3. 실행 예제 B: 배열 내용과 동등성

Program.cs는 내용이 같은 새 배열을 각각 담은 Book 두 개를 비교합니다.
결과는 False입니다.
레코드가 멤버의 동등성 구현을 사용한다고 해서 배열 내용을 자동으로 재귀 비교하는 것은 아닙니다.
일반 배열의 동등성은 배열 인스턴스의 동일성에 기반합니다.

배열 내용 자체를 비교하려면 SequenceEqual 같은 연산을 의도적으로 사용해야 합니다.
복제본의 배열까지 독립시키려면 Tags = [.. original.Tags]처럼 배열 저장소를 복사할 수 있습니다.
단, 배열 원소가 또 다른 참조 형식이면 그 원소 내부까지 깊게 복제되지는 않습니다.
어느 수준의 독립성이 필요한지 자료 구조를 보고 정해야 합니다.

## 4. 실행 예제 C: 관계 패턴과 순서

~~~csharp
static string Grade(int score) => score switch
{
    < 0 or > 100 => "invalid",
    >= 90 => "A",
    >= 80 => "B",
    _ => "C"
};
~~~

switch 식은 값을 입력받아 일치하는 첫 번째 arm의 결과를 반환합니다.
관계 패턴은 <, <=, >, >=로 범위를 표현하고 or는 둘 중 하나를 허용합니다.
and는 두 조건을 모두, not은 패턴에 맞지 않는 경우를 표현합니다.
복잡한 조합은 괄호를 넣어 의도를 분명하게 표현하세요.

잘못된 범위를 먼저 처리해야 150점이 A로 분류되지 않습니다.
기본 arm인 _는 앞선 조건에 맞지 않는 입력을 처리합니다.
컴파일러의 완전성 경고가 도움이 되더라도 유효하지 않은 업무 값의 의미는 개발자가 정해야 합니다.
특히 enum에는 선언된 이름 이외의 숫자값이 들어올 수 있음을 기억하세요.

## 5. 실행 예제 D: 프로퍼티와 목록 패턴

~~~csharp
book switch
{
    { Price: <= 30, Title.Length: > 0 } => "budget",
    _ => "standard"
};
~~~

프로퍼티 패턴은 객체 내부 값을 구조적으로 검사합니다.
쉼표로 나열한 조건은 모두 만족해야 합니다.
Title.Length는 중첩된 프로퍼티 접근을 간단히 표현한 확장 프로퍼티 패턴입니다.
패턴이 프로퍼티를 읽으므로 복잡한 부작용이 있는 getter는 읽기 어려운 동작을 만들 수 있습니다.

~~~csharp
int[] sequence = [1, 2, 3, 4];
bool matches = sequence is [1, .., 4];
~~~

목록 패턴은 원소 위치와 길이 모양을 검사합니다.
앞 원소는 1, 마지막 원소는 4여야 하며 ..는 그 사이 0개 이상의 원소를 허용합니다.
목록 패턴의 [ ]는 데이터를 만드는 컬렉션 식과 모양이 비슷하지만 “검사”하는 문맥입니다.

Describe의 object? 입력에는 null, 정수, 비어 있지 않은 문자열, 그 밖의 경우가 올 수 있습니다.
형식 패턴 int number는 형식 검사와 새 변수 도입을 한 번에 수행합니다.
마지막 _를 두어 새 형식이 들어와도 other로 처리하도록 했습니다.

## 실행과 정확한 기본 출력

~~~powershell
dotnet run --project chapters/20-RecordsPatterns/Chapter20.csproj
dotnet run --project chapters/20-RecordsPatterns/Chapter20.csproj -- --answers
~~~

~~~text
value-equal=True
same-instance=False
titles=C#/C# Guide
shared-tag=advanced
array-content-equality=False
grade=A
price=budget
list-match=True
unknown=other
null=empty
~~~

## 자주 하는 실수와 이유

| 실수 | 이유 | 바르게 생각하기 |
| --- | --- | --- |
| record는 무조건 값 형식이라고 생각하기 | 기본 record는 참조 형식입니다. | 동등성과 저장 방식을 분리합니다. |
| with가 깊은 복사라고 생각하기 | 참조 멤버를 공유할 수 있습니다. | 중첩 자료의 복사 범위를 정합니다. |
| init이면 배열 내용도 불변이라고 생각하기 | 프로퍼티 재대입과 내부 변경은 다릅니다. | 불변 자료 구조를 검토합니다. |
| 넓은 패턴을 먼저 배치하기 | 앞에서 잡히면 뒤의 세부 규칙에 도달하지 않습니다. | 구체적인 조건부터 검토합니다. |
| 기본 arm만 두고 오류 값을 숨기기 | 잘못된 입력도 정상 결과가 됩니다. | 입력 검증 정책을 따로 정의합니다. |

해시 기반 컬렉션의 키로 쓰는 데이터가 변하면 조회가 어려워질 수 있습니다.
키의 동등성과 해시 계산에 영향을 주는 상태는 저장 후 변경하지 않는 편이 안전합니다.

## 연습문제

### 1단계 · 좌표 변경본

Point(1, 2)의 X만 5로 바꾼 새 레코드를 만들고 원본과 변경본을 출력하세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
Point moved = original with { X = 5 };
~~~

정답 실행 결과는 points=1,2/5,2입니다.
원본 변수에 새 값을 대입하는 방식과 원본 객체를 변경하는 방식을 혼동하지 마세요.

</details>

### 2단계 · 나이 분류

음수는 invalid, 18 미만은 child, 65 미만은 adult, 나머지는 senior로 분류하세요.
-1, 12, 20, 70을 검사하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 Category switch 식을 확인하세요.
범위가 작은 조건부터 배치해 앞 조건을 통과한 사실을 다음 조건에서 활용합니다.
18과 65 같은 경계값도 추가로 실행하며 조건 부호를 검증해 보세요.

</details>

### 3단계 · 배열도 독립시키기

Book을 with로 복사하되 복제본 Tags[0] 변경이 원본에 영향을 주지 않게 하세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
Book independent = first with { Tags = [.. first.Tags] };
~~~

새 배열을 만든 뒤 프로퍼티에 넣습니다.
정답 실행에서는 tags=basic/changed가 출력됩니다.
이 배열의 원소는 변경 불가능한 string이므로 이 예제에 필요한 독립성을 얻습니다.

</details>

## 복습 Q&A

**Q. 레코드 두 개가 ==이면 ReferenceEquals도 True인가요?**  
A. 아닙니다. 같은 데이터인 별도 인스턴스일 수 있습니다.

**Q. 배열 프로퍼티가 있는 레코드는 배열 내용을 자동 비교하나요?**  
A. 아닙니다. 해당 멤버 형식이 제공하는 동등성 규칙을 사용합니다.

**Q. switch 식의 _는 언제 필요하나요?**  
A. 나머지 입력을 처리하거나 예외 정책을 명시할 때 사용합니다. 모든 가능한 입력을 고려해야 합니다.

## 공식 문서

- [레코드 형식](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/record)
- [with 식](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/with-expression)
- [패턴 참고](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns)
- [switch 식](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/switch-expression)
