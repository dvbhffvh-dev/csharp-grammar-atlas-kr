# 14. List, Dictionary, HashSet, Queue, Stack

[이전: 13 제네릭](../13-Generics/README.md) · [목차](../../README.md) · [다음: 15 예외와 자원](../15-ExceptionsResources/README.md)

## 시작하기

선수 지식: 배열, 반복문, 메서드, 제네릭 형식입니다.
학습 목표는 데이터의 사용 방식에 맞는 컬렉션을 선택하고 안전하게 조회·수정하는 것입니다.
컬렉션은 언어 키워드가 아니라 .NET이 제공하는 자료구조 형식입니다.
이 프로젝트의 암시적 using 설정에는 일반 제네릭 컬렉션 네임스페이스가 포함됩니다.

저장소 루트에서 실행합니다.

```powershell
dotnet run --project chapters/14-Collections/Chapter14.csproj
dotnet run --project chapters/14-Collections/Chapter14.csproj -- --answers
```

[전체 예제](Program.cs) · [실행 가능한 해답](ExerciseSolutions.cs) · [예상 출력](expected-output.txt)

## 1. 자료구조 선택 기준

| 필요 | 주로 선택할 형식 | 대표 연산 |
|---|---|---|
| 순서와 인덱스, 가변 길이 | `List<T>` | `Add`, `RemoveAt`, 인덱서 |
| 키로 값 조회 | `Dictionary<TKey, TValue>` | `TryGetValue`, 인덱서 |
| 중복 없는 원소와 포함 여부 | `HashSet<T>` | `Add`, `Contains` |
| 먼저 넣은 것을 먼저 처리 | `Queue<T>` | `Enqueue`, `Dequeue` |
| 마지막에 넣은 것을 먼저 처리 | `Stack<T>` | `Push`, `Pop` |

배열의 `Length`와 컬렉션의 `Count`를 구분하세요.
`List<T>.Capacity`는 현재 원소 수가 아니라 확보한 내부 공간의 크기입니다.
대개 업무 로직에서는 `Capacity`보다 `Count`를 사용합니다.
리스트의 인덱스 접근은 빠르지만 중간 삽입·삭제에는 뒤 원소 이동이 필요할 수 있습니다.
해시 기반 컬렉션의 조회는 보통 효율적이지만 해시 충돌과 비교 방식의 영향을 받습니다.

## 2. 예제 A: List의 순서와 변경

```csharp
List<int> scores = new() { 30, 10, 20 };
scores.Add(40);
scores.Remove(10);
Console.WriteLine($"list: {string.Join(", ", scores)}");
```

첫 줄은 30, 10, 20을 순서대로 저장합니다.
`Add(40)`은 뒤에 40을 추가합니다.
`Remove(10)`은 값이 10인 첫 원소를 찾아 삭제합니다.
결과는 30, 20, 40입니다.
`RemoveAt(1)`이라면 값 1이 아니라 인덱스 1의 원소를 삭제합니다.
없는 값을 `Remove`하면 false를 반환하지만 잘못된 인덱스로 접근하면 예외가 발생합니다.

리스트는 중복을 허용하며 자동으로 정렬하지 않습니다.
`Sort()`를 호출하면 현재 리스트 자체의 순서가 변경됩니다.
다른 변수가 같은 리스트를 참조하고 있다면 그 변수에서도 변경된 순서를 보게 됩니다.

## 3. 예제 B: Dictionary의 키와 안전한 조회

```csharp
Dictionary<string, int> stock = new(StringComparer.Ordinal)
{
    ["pen"] = 3,
    ["book"] = 2
};
stock["pen"] += 1;
bool found = stock.TryGetValue("pen", out int count);
Console.WriteLine($"dictionary: {found} / {count}");
Console.WriteLine($"missing: {stock.TryGetValue("eraser", out _)}");
```

사전의 각 항목은 키와 값의 쌍입니다.
여기서는 문자열 키로 정수 수량을 찾습니다.
`StringComparer.Ordinal`은 문화권에 의존하지 않는 대소문자 구분 비교를 선택합니다.
대소문자를 구분하지 않는 식별자를 원하면 `OrdinalIgnoreCase` 등 적절한 비교자를 선택합니다.

`stock["pen"] += 1`은 존재하는 값 3을 읽고 4로 다시 저장합니다.
`TryGetValue`는 존재 여부를 bool로 반환하고 찾은 값을 `out` 변수에 담습니다.
키가 없으면 false이며 `out` 값은 값 형식의 기본값인 0입니다.
따라서 수량이 실제 0인 경우와 키 자체가 없는 경우는 반환 bool로 구분해야 합니다.
`out _`는 찾은 값이 필요 없음을 표현합니다.

없는 키를 인덱서로 **읽으면** 예외가 발생합니다.
인덱서로 **대입하면** 새 키를 추가하거나 기존 값을 덮어씁니다.
`Add`는 중복 키를 허용하지 않고 `TryAdd`는 중복일 때 false를 반환합니다.

## 4. 예제 C: 집합과 처리 순서

```csharp
HashSet<string> tags = new(StringComparer.Ordinal) { "C#", "dotnet" };
Console.WriteLine($"set add: {tags.Add("C#")} / {tags.Add("study")}");
Console.WriteLine($"set count: {tags.Count}");
```

`HashSet`에는 같은 원소가 중복 저장되지 않습니다.
이미 있는 `C#`의 추가는 false를 반환합니다.
새 원소 `study`는 true를 반환하고 총 원소 수는 3이 됩니다.
사전의 키와 집합의 원소는 일관된 동등성 비교 및 해시 규칙을 사용해야 합니다.
객체를 키로 쓸 때 비교 기준이 되는 값을 저장 후 변경하면 조회가 어려워질 수 있습니다.

```csharp
Queue<string> waiting = new();
waiting.Enqueue("first");
waiting.Enqueue("second");
Stack<string> history = new();
history.Push("first");
history.Push("second");
Console.WriteLine($"order: {waiting.Dequeue()} / {history.Pop()}");
```

큐는 FIFO, 즉 먼저 들어온 값이 먼저 나옵니다.
스택은 LIFO, 즉 마지막에 들어온 값이 먼저 나옵니다.
그래서 큐에서는 `first`, 스택에서는 `second`를 꺼냅니다.
`Peek`는 제거 없이 다음 값을 확인하는 메서드입니다.
빈 큐나 스택에서 제거 연산을 호출하면 실패합니다.
실패를 일반적인 상황으로 처리하려면 `TryDequeue`, `TryPop`을 사용할 수 있습니다.

## 5. 예제 D: 반복하며 삭제하기

```csharp
List<int> numbers = new() { 1, 2, 3, 4 };
for (int i = numbers.Count - 1; i >= 0; i--)
{
    if (numbers[i] % 2 == 0)
    {
        numbers.RemoveAt(i);
    }
}
Console.WriteLine($"filtered: {string.Join(", ", numbers)}");
```

`foreach`로 `List<T>`를 열거하는 동안 원소를 추가·삭제하면 열거자가 무효화될 수 있습니다.
컬렉션마다 세부 규칙이 다르므로 임의의 변경을 허용한다고 가정하지 마세요.
여기서는 인덱스를 뒤에서 앞으로 이동하며 삭제합니다.
뒤 원소를 지워도 아직 방문하지 않은 앞쪽 인덱스는 바뀌지 않습니다.
따라서 짝수 4와 2를 안전하게 제거하고 1, 3을 남깁니다.

앞에서부터 제거하면서 인덱스도 증가시키면 당겨진 다음 원소를 건너뛸 수 있습니다.
대안으로 `RemoveAll`이나 별도 결과 컬렉션을 사용할 수 있습니다.
조건을 표현하는 람다는 17단원에서 배웁니다.

## 6. 출력 순서는 자료구조의 계약을 따릅니다

리스트는 인덱스와 순서가 중요하며 큐·스택은 제거 순서가 중요합니다.
사전이나 집합의 순회 결과를 원하는 정렬 순서라고 가정하지 마세요.
정렬된 출력이 필요하면 키나 원소를 정렬한 후 출력해야 합니다.
이 예제는 사전의 특정 키를 조회하고 집합의 개수만 출력하므로 순회 순서에 의존하지 않습니다.

## 7. 전체 기본 출력

```text
list: 30, 20, 40
dictionary: True / 4
missing: False
set add: False / True
set count: 3
order: first / second
filtered: 1, 3
```

## 8. 자주 하는 실수

| 실수 | 해결 |
|---|---|
| `Remove(2)`를 인덱스 삭제로 이해함 | 인덱스 삭제는 `RemoveAt(2)`입니다. |
| 없는 사전 키를 바로 읽음 | `TryGetValue`로 존재 여부를 함께 확인합니다. |
| 리스트가 자동으로 정렬된다고 생각함 | 입력 순서를 유지하며 정렬은 별도 작업입니다. |
| 집합의 출력 순서를 업무 규칙으로 사용함 | 순서가 필요하면 별도 리스트나 정렬 단계를 사용합니다. |
| `foreach` 중 같은 리스트에서 삭제함 | 역방향 인덱스 삭제 또는 별도 결과를 사용합니다. |

## 9. 연습문제

### 문제 1 · 기초: 리스트 정렬

`{ 4, 1, 4, 2 }`를 오름차순으로 정렬하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 1번은 `Sort()`를 호출합니다.
리스트는 중복을 허용하므로 4가 두 번 남습니다.
예상 출력은 `1: 1, 2, 4, 4`입니다.

</details>

### 문제 2 · 응용: 단어 빈도

`red, blue, red`에서 각 단어의 횟수를 사전에 누적하세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 2번은 `TryGetValue`의 기본값 0을 활용합니다.
없던 단어는 0 + 1, 있던 단어는 이전 값 + 1로 대입합니다.
최종 출력은 순회 순서 대신 명시한 키 순서를 사용합니다.
예상 출력은 `2: red=2, blue=1`입니다.

</details>

### 문제 3 · 도전: 첫 등장 순서를 유지한 중복 제거

`3, 1, 3, 2, 1`에서 중복을 제거하되 첫 등장 순서대로 결과를 만드세요.

<details>
<summary>해설</summary>

[해답 파일](ExerciseSolutions.cs)의 3번은 집합과 리스트를 함께 사용합니다.
집합의 `Add`가 true일 때만 결과 리스트에 추가합니다.
포함 여부는 집합이 담당하고 출력 순서는 리스트가 담당합니다.
예상 출력은 `3: 3, 1, 2`입니다.

</details>

## 10. 복습 질문

1. 중복 없는 포함 여부가 중요하면 무엇을 고르나요? → `HashSet<T>`입니다.
2. `TryGetValue`의 bool은 왜 필요한가요? → 키가 없는 경우와 실제 기본값을 저장한 경우를 구분합니다.
3. 역방향 삭제가 유용한 이유는? → 삭제 후 아직 방문하지 않은 앞쪽 인덱스가 바뀌지 않기 때문입니다.

## 공식 문서

- [Microsoft Learn: 컬렉션 선택](https://learn.microsoft.com/en-us/dotnet/standard/collections/selecting-a-collection-class)
- [Microsoft Learn: 제네릭 컬렉션](https://learn.microsoft.com/en-us/dotnet/standard/collections/generic-collections)
