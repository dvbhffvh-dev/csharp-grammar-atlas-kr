# 문법 빠른 찾기

[전체 목차](../README.md)

이 문서는 복습용입니다. 처음 배우는 내용은 연결된 단원의 설명과 실행 예제를 먼저 확인하세요.
아래 짧은 코드는 독립된 프로그램 전체가 아니라 해당 문법을 보여 주는 조각입니다.

## 어떤 문법을 찾고 있나요?

| 하고 싶은 일 | 문법/형식 | 단원 |
|---|---|---|
| 숫자·문자·논리 값을 저장 | `int`, `double`, `decimal`, `char`, `string`, `bool` | [02](../chapters/02-VariablesTypes/README.md) |
| 입력이 정수인지 확인 | `int.TryParse` | [03](../chapters/03-OperatorsConversions/README.md) |
| 조건에 따라 선택 | `if`, `switch`, `?:` | [05](../chapters/05-Conditions/README.md) |
| 같은 동작을 반복 | `for`, `while`, `foreach` | [06](../chapters/06-Loops/README.md) |
| 정해진 수의 요소 보관 | `T[]` | [07](../chapters/07-Arrays/README.md) |
| 동작에 이름을 붙여 재사용 | 메서드, 매개변수, 반환 | [08](../chapters/08-Methods/README.md) |
| 상태와 동작을 묶기 | `class`, 프로퍼티, 생성자 | [10](../chapters/10-ClassesProperties/README.md) |
| 여러 구현을 같은 계약으로 사용 | `interface` | [12](../chapters/12-InterfacesComposition/README.md) |
| 여러 자료형에 공통 코드 적용 | `<T>`, `where` 제약 | [13](../chapters/13-Generics/README.md) |
| 가변 목록·키 검색·중복 제거 | `List<T>`, `Dictionary<K,V>`, `HashSet<T>` | [14](../chapters/14-Collections/README.md) |
| 실패와 자원 정리 처리 | `try`, `catch`, `finally`, `using` | [15](../chapters/15-ExceptionsResources/README.md) |
| 값이 없을 수 있음을 표시 | `int?`, `string?`, `?.`, `??` | [16](../chapters/16-Nullable/README.md) |
| 동작 자체를 전달 | 델리게이트, 람다 `=>` | [17](../chapters/17-DelegatesLambdas/README.md) |
| 변경 사실을 알리기 | `event` | [18](../chapters/18-Events/README.md) |
| 목록에서 조건/정렬/집계 | LINQ | [19](../chapters/19-Linq/README.md) |
| 데이터 중심 모델과 조건 분해 | `record`, `with`, 패턴 | [20](../chapters/20-RecordsPatterns/README.md) |
| 파일에 저장하고 복원 | `File`, `Path`, `JsonSerializer` | [21](../chapters/21-FilesJson/README.md) |
| 완료를 비동기로 기다리기 | `Task`, `async`, `await` | [22](../chapters/22-AsyncAwait/README.md) |

## 헷갈리기 쉬운 기호

| 표현 | 의미 | 구분할 점 |
|---|---|---|
| `x = 3` | 대입 | 값을 변경한다 |
| `x == 3` | 동등 비교 | 비교 규칙은 형식과 연산자 정의에 따라 달라진다 |
| `x is null` | null 패턴 | 사용자 정의 `==`에 의존하지 않는다 |
| `a && b` | 조건부 논리 AND | `a`가 false면 `b`를 평가하지 않는다 |
| `a & b` | 비트 AND 또는 bool 논리 AND | bool일 때도 두 피연산자를 모두 평가한다 |
| `a ?? b` | 널 병합 | `a`가 null일 때만 `b`를 평가한다 |
| `a?.Name` | 널 조건부 접근 | `a`가 null이면 접근을 생략하고 null 결과를 만든다 |
| `x!` | 널 경고 억제 | null을 검사하거나 값을 바꾸지 않는다 |
| `!x` | 논리 NOT | bool 값을 반전한다 |
| `x => x * 2` | 람다 | 입력을 받아 결과를 계산하는 동작이다 |
| `items[^1]` | 끝에서 첫 요소 | 빈 배열이면 범위를 벗어난다 |
| `items[1..3]` | 범위 | 시작 1 포함, 끝 3 제외 |

## `var`, `object`, `dynamic`

`var`는 컴파일러가 초기값으로 **정적 형식**을 추론하는 문법입니다.
한 번 `int`로 정해진 변수에 이후 문자열을 넣을 수 없습니다.
`object`는 형식 이름입니다. 여러 형식의 값을 참조할 수 있지만 멤버를 쓰려면 필요한 형식으로 확인/변환해야 합니다.
`dynamic`은 일부 연산 검사를 런타임에 미룹니다. 존재하지 않는 멤버를 호출하면 실행 중 실패할 수 있습니다.
입문 예제에서 `dynamic`을 이용해 형식 오류를 피하려 하기보다 형식과 변환 규칙을 먼저 익히세요.

```csharp
var count = 3;              // 컴파일 시 int
object value = "C#";
if (value is string text)
    Console.WriteLine(text.Length);
```

## 값 복사와 객체 복사

값 형식 변수 대입은 그 값을 복사합니다. 참조 형식 변수 대입은 같은 객체를 가리키는 참조를 복사합니다.
`struct` 안에 참조 필드가 있다면 구조체를 복사해도 그 필드가 가리키는 객체는 공유할 수 있습니다.
`string`은 참조 형식이지만 불변입니다. 문자열을 변경하는 것처럼 보이는 연산은 새 문자열을 만들 수 있습니다.
`record`의 `with`도 참조 멤버까지 자동으로 깊게 복제하지 않습니다.
메모리 위치를 “값 형식은 무조건 스택, 참조 형식은 무조건 힙”으로 외우면 잘못된 결론에 이릅니다.

## `const`, `readonly`, `init`, `required`

| 문법 | 목적 | 주의 |
|---|---|---|
| `const` | 컴파일 시 상수 | 상수로 표현 가능한 형식과 식으로 제한 |
| `readonly` 필드 | 필드 재대입 시점을 제한 | 참조 대상 객체 전체를 불변으로 만들지 않음 |
| `init` 프로퍼티 | 초기화 시에만 일반 대입 허용 | 멤버가 참조하는 객체는 가변일 수 있음 |
| `required` 멤버 | 호출자가 초기화하도록 컴파일러가 요구 | 값의 범위·빈 문자열을 자동 검증하지 않음 |

## 접근 제한자

| 제한자 | 일반적인 접근 범위 |
|---|---|
| `public` | 해당 형식에 접근 가능한 외부 코드 |
| `private` | 선언한 형식 내부 |
| `protected` | 선언한 클래스와 파생 클래스에 허용되는 접근 |
| `internal` | 같은 어셈블리 |
| `protected internal` | 같은 어셈블리 **또는** 파생 형식 |
| `private protected` | 같은 어셈블리 안의 파생 형식에 제한 |

멤버의 접근 가능성은 멤버를 포함하는 형식의 접근 가능성보다 넓어질 수 없습니다.
세부적인 protected 접근 규칙은 상속과 함께 학습하세요.

## 메서드 인수 전달

```csharp
static void Increase(int value) => value++;       // 호출자의 int를 바꾸지 않음
static void IncreaseRef(ref int value) => value++;

int number = 5;
Increase(number);       // 여전히 5
IncreaseRef(ref number); // 6
```

기본 전달은 값 전달입니다. 참조 형식이라면 **참조 값 자체를 값으로 전달**합니다.
따라서 전달받은 객체의 내부 변경은 보일 수 있지만 매개변수에 다른 객체를 대입하는 것은 호출자 변수를 재대입하지 않습니다.
`ref`는 읽기/쓰기 참조 전달, `out`은 메서드가 값을 할당하여 돌려주는 참조 전달,
`in`은 읽기 전용 참조 전달을 표현합니다. `in`이 모든 경우에 빠른 것은 아닙니다.

## LINQ와 비동기의 실행 시점

```csharp
IEnumerable<int> query = numbers.Where(n => n > 0); // 보통 아직 요소를 순회하지 않음
int[] snapshot = query.ToArray();                  // 여기서 순회하여 배열로 저장
```

`Where`, `Select` 같은 연산은 보통 지연 실행됩니다. `ToArray`, `ToList`, `Count` 등은 결과를 구하기 위해 실행합니다.
원본 컬렉션이 바뀌면 다음 순회 결과가 바뀔 수 있습니다. 모든 LINQ 메서드가 같은 방식으로 동작하지는 않습니다.

`async` 메서드는 호출되면 실행을 시작하며, 미완료 작업을 `await`하는 지점에서 호출자에게 제어를 돌려줄 수 있습니다.
`await`는 “언제나 멈춤”이나 “새 스레드 생성”이라는 뜻이 아닙니다. 이미 완료된 작업이라면 이어서 진행할 수 있습니다.
`Task.WhenAll`은 여러 작업의 완료를 묶어 기다립니다. 입력한 작업의 동시 실행 방식은 각 작업의 구현에 달려 있습니다.

## 추가로 찾아볼 심화 주제

속성(`attribute`)은 코드 요소에 메타데이터를 붙이고, 리플렉션은 실행 중 형식 정보를 살펴보는 기능입니다.
`partial`은 하나의 형식 정의를 여러 파일로 나눌 때 사용합니다. `sealed`는 상속/재정의를 제한할 때 사용합니다.
`Span<T>`는 연속된 메모리 영역을 표현하는 형식이며 수명·참조 안전성 규칙을 함께 배워야 합니다.
`unsafe`, 포인터, 사용자 정의 연산자, 공변/반공변, 식 트리는 핵심 문법을 익힌 뒤 목적에 따라 공부하세요.
이 항목들은 본문의 실행 실습 범위에 포함하지 않습니다.
