# 24장 · C# 12~14의 실용 문법

[← 23장 확장 문법](../23-AdvancedSyntax/README.md) · [전체 목차로 돌아가기](../../README.md)

## 먼저 알아둘 내용

컬렉션, 프로퍼티, 생성자, nullable, 람다, 범위를 먼저 익히세요.
이 장은 C# 12, 13, 14에서 도입된 기능 가운데 코드 읽기에 자주 도움이 되는 기능을 선택합니다.
모든 새 기능을 나열하는 장은 아닙니다.
예제는 저장소에 지정된 .NET 10 및 C# 14 설정으로 컴파일하도록 작성했습니다.
언어 버전과 .NET 실행 환경 버전은 관련되지만 같은 개념은 아닙니다.

## 학습 목표

- 컬렉션 식의 대상 형식과 펼치기 요소를 이해합니다.
- 일반 클래스의 주 생성자와 record의 위치 기반 선언을 구분합니다.
- C# 13의 params 컬렉션을 읽고 작성합니다.
- C# 14의 field 기반 프로퍼티로 검증을 수행합니다.
- null 조건부 대입에서 오른쪽 식의 실행 여부를 예측합니다.

## 1. 버전을 먼저 확인하기

프로젝트의 TargetFramework는 대상으로 하는 .NET API와 실행 환경을 정합니다.
LangVersion은 컴파일러가 허용하는 C# 문법 버전을 정합니다.
SDK와 IDE가 해당 문법을 이해해야 편집기 오류와 실제 빌드 결과도 일치합니다.
구형 Visual Studio에서 프로젝트를 열기만 했다고 최신 언어 기능을 사용할 수 있는 것은 아닙니다.

이 저장소는 Directory.Build.props에서 net10.0과 C# 14.0을 명시합니다.
이 장의 문법을 이전 프로젝트로 복사할 때는 그 프로젝트의 설정을 확인하세요.
무조건 preview를 켜기보다 실제 사용하는 기능의 정식 지원 버전을 확인하는 습관이 좋습니다.

| 기능 | 도입 버전 | 이 장의 용도 |
| --- | --- | --- |
| 컬렉션 식 [ ... ] | C# 12 | 배열과 리스트 초기화 |
| 일반 클래스·구조체 주 생성자 | C# 12 | 생성 인수를 선언부에 표현 |
| params 컬렉션 확장 | C# 13 | ReadOnlySpan<int> 가변 인수 |
| field 기반 프로퍼티 | C# 14 | 자동 저장소에 검증 추가 |
| null 조건부 대입 | C# 14 | null이면 대입과 오른쪽 계산 생략 |

field는 이전 버전에서 실험적으로 소개된 이력이 있지만 이 교재는 C# 14의 정식 기능으로 사용합니다.
버전별 전체 기능은 마지막의 Microsoft Learn 문서를 참고하세요.

## 2. 실행 예제 A: 컬렉션 식과 펼치기

~~~csharp
int[] first = [1, 2];
List<int> combined = [0, .. first, 3];
~~~

컬렉션 식은 원하는 컬렉션의 내용을 간결하게 표현합니다.
왼쪽의 대상 형식이 int[]이면 배열을, List<int>이면 리스트를 만들 수 있습니다.
.. first는 first의 원소들을 현재 컬렉션에 차례로 넣는 펼치기 요소입니다.
결과는 0,1,2,3입니다.

[ ]만 보고 항상 배열이라고 판단하지 마세요.
컴파일러는 주변 문맥의 대상 형식을 사용합니다.
대상 형식이 없는 var numbers = [1, 2] 형태는 이 장의 설정에서 사용할 수 없습니다.
int[] numbers = [1, 2]처럼 형식을 지정하세요.

컬렉션 식은 결과 컬렉션을 만들기 위해 펼칠 시퀀스를 열거합니다.
LINQ의 지연 쿼리를 새 컬렉션에 펼치면 그 시점에 원소를 소비합니다.
참조 형식 원소를 펼친다고 객체 내부까지 복제되는 것은 아닙니다.

목록 패턴 [1, .., 4]는 데이터를 검사하고, 컬렉션 식 [1, .. values, 4]는 데이터를 만듭니다.
같은 기호라도 표현이 놓인 문맥과 역할을 구분하세요.
Range의 1..3 역시 별도의 용법입니다.

## 3. 실행 예제 B: 일반 클래스의 주 생성자

~~~csharp
public sealed class Lesson(string title, int number)
{
    public string Label { get; } = $"{number}:{title}";
}
~~~

주 생성자는 형식 이름 뒤에 생성에 필요한 매개변수를 직접 씁니다.
이 매개변수들은 형식 본문에서 사용할 수 있습니다.
일반 class의 title과 number가 자동으로 공개 프로퍼티가 되는 것은 아닙니다.
예제는 공개하고 싶은 결과를 Label 프로퍼티로 명시했습니다.

~~~csharp
Lesson lesson = new("LINQ", 19);
Console.WriteLine(lesson.Label);
~~~

new Lesson()이 자동으로 가능한 것은 아닙니다.
필요한 생성 인수를 제공해야 하며 별도 생성자를 추가한다면 주 생성자로 연결해야 합니다.
인스턴스 멤버에서 매개변수를 계속 사용하면 컴파일러가 필요한 상태를 보관할 수 있습니다.
어떤 저장소가 생길지 추측해 코드를 최적화하기보다 공개 상태와 변경 의도를 명확하게 설계하세요.

record Lesson(string Title, int Number)는 위치 기반 프로퍼티와 값 동등성 등 다른 기능도 제공합니다.
일반 class에 괄호를 붙이는 것만으로 레코드의 모든 동작이 생기지 않습니다.

## 4. 실행 예제 C: params 컬렉션

~~~csharp
static int Total(params ReadOnlySpan<int> values)
{
    int total = 0;
    foreach (int value in values)
    {
        total += value;
    }
    return total;
}
~~~

기존의 params int[]에 익숙하다면 호출 모양은 같습니다.
Total(2, 3, 5)처럼 값을 나열하거나 인수가 없는 호출을 사용할 수 있습니다.
C# 13부터 params가 지원하는 대상이 배열 외의 특정 컬렉션 형식으로 확장되었습니다.
이 예제에서는 연속된 읽기 전용 메모리 구간을 표현하는 ReadOnlySpan<int>를 사용합니다.

params는 마지막 매개변수에만 하나 사용할 수 있습니다.
빈 인수 목록도 유효하므로 메서드는 빈 입력의 의미를 정해야 합니다.
이 합계 예제는 빈 입력이면 초기값 0을 반환합니다.

모든 IEnumerable<T> 구현이 자동으로 params 대상이 되는 것은 아닙니다.
지원되는 형식과 생성·추가 계약이 정해져 있으므로 참조 문서를 확인하세요.
배열과 span을 받는 오버로드를 함께 둘 때는 오버로드 선택에 영향을 줄 수 있습니다.
새 문법으로 바꾸기 전에 공개 API 호출 결과도 검증해야 합니다.

## 5. 실행 예제 D: 프로퍼티의 field와 조건부 대입

~~~csharp
public string Name
{
    get;
    set => field = string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Name must not be blank.", nameof(value))
        : value.Trim();
} = "unknown";
~~~

자동 프로퍼티는 컴파일러가 내부 저장 필드를 만듭니다.
field는 프로퍼티 접근자 안에서 그 저장소를 가리키는 문맥 키워드입니다.
별도 _name 필드를 선언하지 않고도 setter에 정리와 검증을 추가할 수 있습니다.
get;은 같은 저장소를 읽는 자동 getter입니다.

초기값 unknown은 새 객체가 기본적으로 유효한 이름을 갖도록 합니다.
"  Ada  "를 대입하면 Trim 후 Ada를 저장합니다.
공백뿐인 문자열은 예외를 발생시키며 이전 정상값을 덮어쓰지 않습니다.
field와 같은 이름의 기존 식별자가 있는 코드에서는 의미 충돌을 검토해야 합니다.

C# 14는 참조 형식에 대한 null 조건부 접근을 대입 왼쪽에 사용할 수 있게 합니다.

~~~csharp
ReaderProfile? missing = null;
missing?.Name = BuildName();
~~~

missing이 null이면 setter뿐 아니라 BuildName 호출도 건너뜁니다.
Program.cs는 rightSideCalls를 증가시키는 함수로 이 규칙을 관찰합니다.
null인 객체에서는 skipped=0이며, 존재하는 객체에 대입한 후에는 calls=1입니다.

이 문법이 null인 객체를 자동으로 생성해 주지는 않습니다.
존재할 때만 변경하고, 없으면 아무것도 하지 않는 정책을 표현합니다.
객체가 없다는 것이 오류라면 조용히 건너뛰는 대신 검증 또는 예외 처리가 맞을 수 있습니다.
null 조건부 대입은 ++와 --까지 허용하는 규칙은 아니며 그 경우는 별도 코드를 사용하세요.

## 실행과 정확한 기본 출력

~~~powershell
dotnet run --project chapters/24-ModernCSharp/Chapter24.csproj
dotnet run --project chapters/24-ModernCSharp/Chapter24.csproj -- --answers
~~~

~~~text
collection=0,1,2,3
primary=19:LINQ
params=10
field=Ada
skipped=0
assigned=Ben,calls=1
~~~

## 자주 하는 실수와 이유

| 실수 | 실제 의미 | 수정 방향 |
| --- | --- | --- |
| var에 컬렉션 식만 대입하기 | 대상 컬렉션 형식이 필요합니다. | 배열·리스트 등의 형식을 명시합니다. |
| 일반 주 생성자에 자동 프로퍼티를 기대하기 | record와 생성 규칙이 다릅니다. | 공개 프로퍼티를 직접 선언합니다. |
| params에 임의 형식을 사용하기 | 지원되는 컬렉션 계약이 있습니다. | 지원 목록과 호출 규칙을 확인합니다. |
| field를 일반 지역 변수처럼 쓰기 | 프로퍼티 접근자의 저장소 기능입니다. | 유효한 문맥에서만 사용합니다. |
| ?. 대입이 객체도 만든다고 생각하기 | null이면 전체 대입을 생략합니다. | 생성이 필요하면 먼저 객체를 만듭니다. |

## 연습문제

### 1단계 · 양끝 값 붙이기

[2, 3]을 펼쳐서 [1, 2, 3, 4]인 새 배열을 만드세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
int[] all = [1, .. middle, 4];
~~~

대상 형식 int[]과 펼치기 요소 .. middle을 함께 확인하세요.
실행 정답은 spread=1,2,3,4입니다.

</details>

### 2단계 · 빈 params 입력

ReadOnlySpan<int>를 받는 Sum을 작성하고 Sum(3, 7)과 Sum()을 호출하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 Sum이 완전한 코드입니다.
초기값 0에서 각 값을 더하므로 결과는 sum=10, empty-sum=0입니다.
빈 입력을 거부해야 하는 다른 업무 함수라면 이 위치에 명시적인 검증이 필요합니다.

</details>

### 3단계 · 조건부 대입과 문자열 정리

null인 ReaderProfile 변수에 이름을 대입해 보고, 객체를 만든 뒤 "  Cora  "를 대입하세요.

<details>
<summary>정답과 해설</summary>

첫 대입 후에도 변수는 null이며 still-null=True를 출력합니다.
객체 생성 후에는 setter가 호출되어 trimmed=Cora가 출력됩니다.
null 검사와 setter 내부 정리는 서로 다른 두 단계입니다.

</details>

## 복습 Q&A

**Q. 컬렉션 식 [1, 2] 자체의 형식은 항상 int[]인가요?**  
A. 아닙니다. 주변의 대상 형식에 맞춰 변환됩니다.

**Q. 일반 클래스의 주 생성자는 자동 프로퍼티를 만들까요?**  
A. 아닙니다. 필요한 공개 멤버를 직접 선언합니다.

**Q. x?.Name = Expensive()에서 x가 null이면 Expensive가 실행되나요?**  
A. 실행되지 않습니다. 오른쪽 식도 생략됩니다.

## 다음 학습으로 이어가기

이제 작은 프로그램을 만들며 형식, 컬렉션, 예외, LINQ, 파일 저장, 비동기를 함께 사용해 보세요.
예를 들어 학습 진도 목록을 레코드로 표현하고 LINQ로 집계한 뒤 JSON에 저장할 수 있습니다.
처음부터 모든 최신 문법을 넣기보다 각 문법이 의도를 더 분명하게 만드는지 판단하세요.
실행 결과를 예측하고 디버거로 확인하는 과정을 반복하면 문법이 실제 설계 도구로 연결됩니다.

## 공식 문서와 확인 기준

이 장의 버전 표와 문법은 2026-10-29에 Microsoft Learn의 정식 기능 문서를 기준으로 확인했습니다.

- [C# 12의 새로운 기능](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-12)
- [C# 13의 새로운 기능](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-13)
- [C# 14의 새로운 기능](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14)
- [컬렉션 식](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions)
- [주 생성자](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/primary-constructors)
- [params 매개변수](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters)
- [멤버 접근과 null 조건부 연산자](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators)
