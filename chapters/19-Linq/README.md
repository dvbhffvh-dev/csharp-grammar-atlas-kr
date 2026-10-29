# 19장 · LINQ와 지연 실행

[← 18장 이벤트](../18-Events/README.md) · [전체 목차](../../README.md) · [20장 레코드와 패턴 →](../20-RecordsPatterns/README.md)

## 먼저 알아둘 내용

배열, List<T>, IEnumerable<T>, 람다와 제네릭을 먼저 익히세요.
LINQ는 반복문을 없애는 마법이 아니라 데이터를 조회하고 변환하는 연산의 모음입니다.
이 장의 예제는 메모리 안의 컬렉션에 적용되는 LINQ to Objects입니다.
데이터베이스 공급자의 IQueryable<T>는 번역 가능한 식과 실행 방식에 별도 제약이 있습니다.

## 학습 목표

- Where와 Select의 역할을 구분합니다.
- 정렬, 그룹화, 조인, 존재 여부 확인을 조합합니다.
- 쿼리 선언과 실제 열거 시점을 구분합니다.
- 반복 열거의 비용과 부작용을 설명합니다.
- ToArray와 ToList로 원하는 시점의 결과를 구체화합니다.

## 1. 데이터 흐름을 단계별로 읽기

LINQ 연산은 원본 시퀀스를 받아 결과 또는 새 시퀀스를 만듭니다.
Where는 조건을 통과하는 항목만 남기고 Select는 각 항목을 다른 값으로 바꿉니다.
학생 객체에서 이름만 얻는 Select를 실행하면 결과 원소 형식은 Student가 아니라 string입니다.
일반적인 조회 연산 자체는 원본 배열이나 리스트를 정렬하거나 삭제하지 않습니다.
다만 람다 본문에서 원본 객체를 변경하면 그 부작용은 그대로 일어날 수 있습니다.

| 연산 | 질문 | 결과 |
| --- | --- | --- |
| Where | 어떤 항목을 남길까? | 조건에 맞는 시퀀스 |
| Select | 각 항목을 무엇으로 바꿀까? | 변환된 시퀀스 |
| OrderBy | 어떤 키로 정렬할까? | 정렬된 시퀀스 |
| GroupBy | 같은 키끼리 묶을까? | 그룹의 시퀀스 |
| Join | 두 시퀀스를 어떤 키로 연결할까? | 짝지은 결과의 시퀀스 |
| Any | 하나라도 존재할까? | bool |
| ToArray | 지금 결과를 저장할까? | 배열 |

여러 정렬 기준이 필요하면 OrderBy 뒤에 ThenBy를 사용합니다.
OrderBy를 다시 호출하면 기존 정렬을 보조하는 것이 아니라 새로운 주 정렬을 적용합니다.

## 2. 실행 예제 A: 통과한 학생의 이름

~~~csharp
string[] names = students
    .Where(student => student.Score >= 80)
    .OrderBy(student => student.Name, StringComparer.Ordinal)
    .Select(student => student.Name)
    .ToArray();
~~~

먼저 80점 이상인 Cora와 Ada를 남깁니다.
이름을 기준으로 정렬하면 Ada, Cora가 됩니다.
마지막에 이름만 추출하고 배열에 저장합니다.
StringComparer.Ordinal은 문자열 코드 값 기준의 비교를 명시해 실행 문화권에 따른 차이를 줄입니다.

각 단계에서 “현재 원소 하나의 형식이 무엇인가?”를 질문하면 긴 연결도 읽기 쉬워집니다.
Where와 OrderBy 단계의 원소는 Student이며 Select 이후의 원소는 string입니다.
ToArray는 시퀀스를 실제로 열거해서 결과를 저장하는 시점입니다.

## 3. 실행 예제 B: 그룹과 조인

GroupBy(student => student.Team)는 팀 이름이 같은 학생들을 묶습니다.
각 그룹에는 Key와 그 그룹에 속한 원소들이 있습니다.
예제는 그룹 키를 정렬한 뒤 Count()를 호출하여 blue 2명, red 1명을 출력합니다.
그룹을 여러 번 계산해야 한다면 원본 크기와 열거 비용을 고려하세요.

~~~csharp
var joined = students.Join(
    teams,
    student => student.Team,
    team => team.Id,
    (student, team) => $"{student.Name}:{team.Title}");
~~~

Join은 학생의 Team과 팀의 Id가 같은 쌍을 연결합니다.
양쪽에서 선택하는 키의 형식과 의미가 맞아야 합니다.
위 코드는 내부 조인입니다. 일치하는 팀이 없는 학생은 결과에서 빠집니다.
일치하는 키가 여러 개 있으면 한 학생이 여러 결과로 나타날 수 있습니다.

예제의 팀 이름은 blue와 red이며 표시용 제목은 Ocean과 Fire입니다.
학생에 표시 제목을 복사해 두는 대신 조회할 때 연결하는 상황을 보여 줍니다.
이 데이터의 Join 결과는 바깥쪽 students의 순서를 유지합니다.

Any(student => student.Score == 100)는 만점자가 하나라도 있는지 확인합니다.
존재만 알고 싶을 때 Count() > 0보다 의도를 직접 표현하고 조건을 만족하면 탐색을 멈출 수 있습니다.

## 4. 실행 예제 C: 선언한 쿼리는 결과 목록이 아니다

~~~csharp
List<int> numbers = [1, 2, 3];
IEnumerable<int> live = numbers.Where(number => number >= 3);
numbers.Add(4);
int[] snapshot = live.ToArray();
numbers.Add(5);
~~~

live를 선언한 순간 Where의 결과가 배열로 저장되는 것이 아닙니다.
실제 항목이 필요해져 열거될 때 원본을 읽고 조건을 평가합니다.
4를 추가한 뒤 ToArray를 호출하므로 snapshot은 3과 4를 담습니다.
이후 원본에 5를 추가해도 snapshot에는 영향이 없습니다.
다시 live를 열거하면 현재 원본을 사용하므로 3,4,5를 얻습니다.

이것이 지연 실행입니다.
지연 실행은 필요한 시점까지 계산을 미룰 수 있지만 원본과 캡처 변수의 변화도 반영할 수 있습니다.
“선언 당시의 목록”이 필요하다면 그 시점에 ToArray나 ToList를 호출하세요.
단, 참조 형식 원소를 담은 배열을 만들었다고 원소 객체까지 깊은 복사되는 것은 아닙니다.

## 5. 실행 예제 D: 두 번 열거하면 두 번 검사한다

Program.cs의 observed는 조건식이 호출될 때마다 predicateCalls를 증가시킵니다.
numbers에는 다섯 항목이 있고 Count를 두 번 수행하므로 검사 횟수는 10입니다.
관찰용으로 부작용을 넣은 예제이며 실제 필터에서는 가능하면 순수한 조건식을 사용하세요.

Where처럼 항목을 하나씩 흘려보내는 연산과 OrderBy처럼 정렬을 위해 원본을 모아야 하는 연산은 다릅니다.
“지연 실행”은 “항상 원소 하나만 읽는다”는 뜻이 아닙니다.
Count와 Any 같은 최종 연산도 원본의 형식이나 조건 유무에 따라 최적화 방식이 달라질 수 있습니다.
따라서 쿼리 안의 부작용이 정확히 몇 번 실행될 것이라는 기대에 업무 로직을 의존시키지 마세요.

## 실행과 정확한 기본 출력

저장소 루트에서 실행합니다.

~~~powershell
dotnet run --project chapters/19-Linq/Chapter19.csproj
dotnet run --project chapters/19-Linq/Chapter19.csproj -- --answers
~~~

~~~text
passed=Ada,Cora
team=blue,count=2
team=red,count=1
joined=Cora:Fire,Ben:Ocean,Ada:Ocean
perfect=False
snapshot=3,4
live=3,4,5
predicate-calls=10
~~~

## 자주 하는 실수와 이유

| 실수 | 왜 결과가 다른가 | 수정 방향 |
| --- | --- | --- |
| Where가 원본에서 항목을 삭제한다고 생각하기 | 필터된 시퀀스를 반환합니다. | 반환 결과를 사용합니다. |
| Select를 필터처럼 사용하기 | 각 항목을 변환할 뿐 제거하지 않습니다. | 조건 제거에는 Where를 사용합니다. |
| 쿼리 변수 이름만 보고 저장된 결과라고 믿기 | 대부분의 시퀀스 연산은 지연 실행합니다. | 저장 시점을 ToArray로 명시합니다. |
| 열거 중 같은 List를 수정하기 | 열거자가 변경을 감지해 예외를 낼 수 있습니다. | 별도 결과를 만든 뒤 수정합니다. |
| ToList를 모든 단계마다 호출하기 | 불필요한 할당과 전체 열거가 늘어납니다. | 결과 재사용이나 시점 고정이 필요한 곳에만 둡니다. |

## 연습문제

### 1단계 · 짝수 제곱 목록

[5, 2, 8, 1, 4]에서 짝수만 골라 오름차순 정렬 후 제곱한 배열을 만드세요.

<details>
<summary>정답과 해설</summary>

~~~csharp
var result = values.Where(value => value % 2 == 0)
    .OrderBy(value => value)
    .Select(value => value * value)
    .ToArray();
~~~

정답 파일은 squares=4,16,64를 출력합니다.
이 예제에서는 입력이 양수이므로 제곱 전 정렬과 제곱 후 정렬의 순서가 같지만 모든 변환에서 그런 것은 아닙니다.

</details>

### 2단계 · 팀별 합계

A팀 90점, B팀 70점, A팀 80점 학생을 GroupBy로 묶고 팀별 점수 합계를 출력하세요.

<details>
<summary>정답과 해설</summary>

ExerciseSolutions.cs의 두 번째 예제를 실행하세요.
그룹 원소에 Sum(student => student.Score)를 적용하면 A는 170, B는 70입니다.
전체 학생의 Sum과 그룹별 Sum은 결과의 의미가 다릅니다.

</details>

### 3단계 · 변하지 않는 결과 만들기

[1, 2, 3]에서 1보다 큰 값을 선택하고, 원본에 4를 추가한 뒤에도 선택 결과가 2,3을 유지하도록 하세요.

<details>
<summary>정답과 해설</summary>

원본 수정 전에 Where(...).ToArray()를 호출합니다.
정답 파일의 fixedResult가 이 시점을 명시합니다.
같은 코드를 ToArray 없이 IEnumerable<int> 변수에 저장해서 실행하면 4도 포함되는지 비교하세요.

</details>

## 복습 Q&A

**Q. LINQ 쿼리를 변수에 대입하면 바로 전부 실행되나요?**  
A. 연산에 따라 다릅니다. Where는 보통 열거할 때 계산하고 ToArray는 즉시 열거합니다.

**Q. Any는 항상 모든 항목을 검사하나요?**  
A. 조건을 만족하는 항목을 찾으면 탐색을 마칠 수 있습니다.

**Q. ToArray를 호출하면 Student 객체들도 복제되나요?**  
A. 아닙니다. 배열 저장소는 새로 생기지만 참조 원소는 같은 객체를 가리킬 수 있습니다.

## 공식 문서

- [LINQ 개요](https://learn.microsoft.com/dotnet/csharp/linq/)
- [쿼리 실행 분류](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries)
- [지연 실행](https://learn.microsoft.com/dotnet/standard/linq/deferred-execution-lazy-evaluation)
- [Enumerable 연산](https://learn.microsoft.com/dotnet/api/system.linq.enumerable)
